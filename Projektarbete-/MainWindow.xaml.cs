using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Projektarbete
{
    // TODO: Spara varukorg till senare. Den är automatiskt sparad i Temp? Så: Läsa in Cart vid Start?
    // Koppla bild till objekt

    public class DepecheModeAlbum 
    {
        public string Title;
        public string Description;
        public decimal Price;
        public string Image;
        //public Image Image;

        // instansmetod/-er
    }
    public partial class MainWindow : Window
    {
        TextBlock myTextblock;
        ComboBox cb;
        ComboBox amountCombobox;
        // checkboxes

        public const string ProductFilePath = "Products.csv"; // inte temp
        public const string CartPath = @"C:\Windows\Temp\Cart.csv"; // temp
        public const string DiscountPath = "DiscountCodes.csv"; // inte temp

        List<DepecheModeAlbum> albumList = new List<DepecheModeAlbum>(); //private el public?
        public Dictionary<DepecheModeAlbum, int> Cart;// = new Dictionary<DepecheModeAlbum, int>(); // tagit bort static
        // Sortimentet (läst från fil):
        DepecheModeAlbum[] produktArray;

        string[] linesProduct = File.ReadAllLines(ProductFilePath); // borde denna vara i Start()?
        string title, description; decimal price;// borde dessa vara i Start()?

        string[] linesDiscount = File.ReadAllLines(DiscountPath);
        string code; decimal percentage; // "never used"... Hamnar inte i Dicitonary för rabattkoderna...

        string[] imagePaths = { "Images/ss.jpg", "Images/abf.jpg", "Images/cta.jpg", "Images/sgr.jpg", "Images/bc.jpg", "Images/mftm.jpg", "Images/violator.jpg", "Images/sofad.jpg", "Images/ultra.jpg", "Images/exciter.jpg", "Images/Images/pta.jpg", "Images/sotu.jpg", "Images/dm.jpg", "Images/spirit.jpg" };



        // Variabler som kommer att behövas:
        string s = "Hur många vill du köpa?";
        int userAmountChoice = 0;
        string ss = "Klicka i vilken/vilka du vill ta bort från varukorgen";
        int[] userRemoveChoice = null; // ska vara lista? Annars "Vill du ta bort en? Flera?"
        string sss = "Klicka i vilken rabattkod du vill använda!";
        string ssss = "Klicka i denna ruta om du vill spara varukorgen till senare!";
        int userDiscountChoice = 0;
        decimal totalSumInclDiscount, totalDiscount, sum;
        //string receipt =  + totalSumInclDiscount + totalDiscount;



        // "We store the saved shopping cart in a CSV file outside the project directory, because then it will not be overwritten everytime we start the program."


        public MainWindow()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            InitializeComponent();
            Start();
        }

        private void Start()
        {
            // test
            string testImage = imagePaths[0]; // ? (0 är Speak & Spell)
            for (int i = 0; i < imagePaths.Length; i++)
            {
                testImage = imagePaths[i];
            }//

            foreach (string line in linesProduct)
            {
                string[] parts = line.Split(',');
                title = parts[0];
                description = parts[1];
                price = decimal.Parse(parts[2]);

                albumList.Add(new DepecheModeAlbum
                {
                    Title = title,
                    Description = description,
                    Price = price,
                    Image = testImage // ? Detta ska inte ligga i loopen för csv
                });
            }
            produktArray = albumList.ToArray(); // Färdiga sortimentet. Listan är bara för filläsningen.


            // Läsa rabattkoder:
            Dictionary<string, decimal> discountCodes = new Dictionary<string, decimal>();// new
            foreach (string lineDiscount in linesDiscount)
            {
                string[] parts = lineDiscount.Split(',');
                code = parts[0];
                percentage = decimal.Parse(parts[1]); // funkar
                
                foreach (KeyValuePair<string, decimal> pair in discountCodes)
                {
                    code = pair.Key;
                    percentage = pair.Value;
                } // discountCodes är tom
            }








            // Window options
            Title = "The DEPECHE MODE record shop";
            Width = 1000; // Ändra eventuellt beroende på innehåll
            Height = 600;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Scrolling
            ScrollViewer root = new ScrollViewer();
            root.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            Content = root;

            // Main grid
            Grid grid = new Grid();
            root.Content = grid;
            grid.Margin = new Thickness(5);

            // ALBUMINFOGRID
            Grid albumInfoGrid = new Grid();
            grid.Children.Add(albumInfoGrid);
            Grid.SetRow(albumInfoGrid, 0);
            Grid.SetColumn(albumInfoGrid, 1);

            albumInfoGrid.RowDefinitions.Add(new RowDefinition());
            albumInfoGrid.RowDefinitions.Add(new RowDefinition());
            albumInfoGrid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            //MAIN
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());

            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            // I ALBUMINFOGRID:
            TextBlock albumInfo = new TextBlock
            {
                Text = "" + produktArray[0].Title + "med utgivningsår: " + produktArray[0].Description + "till priset av: " + produktArray[0].Price + "kr.",
                TextWrapping = TextWrapping.Wrap
            };
            albumInfoGrid.Children.Add(albumInfo);
            Grid.SetRow(albumInfo, 0);
            Grid.SetColumn(albumInfo, 0);




            // Antal:
            amountCombobox = new ComboBox(); // TODO default-text
            amountCombobox.Items.Add("1");
            amountCombobox.Items.Add("2");
            amountCombobox.Items.Add("3");
            amountCombobox.SelectedIndex = 0;
            amountCombobox.SelectionChanged += AmountCombobox_SelectionChanged;

            albumInfoGrid.Children.Add(amountCombobox);
            Grid.SetRow(amountCombobox, 2);
            Grid.SetColumn(amountCombobox, 0);




            // "Create a combobox and then loop over the list of people to populate it."
            cb = new ComboBox();
            foreach (DepecheModeAlbum item in albumList) // albumList/produktArray?
            {
                cb.Items.Add(item.Title); // Alla kommer med
            }
            grid.Children.Add(cb);
            Grid.SetRow(cb, 0);
            Grid.SetColumn(cb, 3);

            // En event handler som sker när användaren väljer något i Comboboxen
            cb.SelectionChanged += ComboboxVald;

            // Add checkboxes for each albumObject: (lämpligt som metod?)

            //CheckBox check = new CheckBox{Content=albumList[0].Title +"my content"}; // verkar funka
            //grid.Children.Add(check);
            //Grid.SetRow(check, 0);
            //Grid.SetColumn(check, 0);

            // "Put some checkboxes in a wrap panel, which moves them to the next line if there is not enough horizontal space."
            StackPanel radioPanel = new StackPanel { Orientation = Orientation.Vertical };
            grid.Children.Add(radioPanel);
            Grid.SetRow(radioPanel, 0);
            Grid.SetColumn(radioPanel, 0);

            // "Create three checkboxes and assign the same event handler to all of them."
            for (int i = 1; i <= produktArray.Length; i++)
            {
                CheckBox myCheckbox = new CheckBox
                {
                    Content = produktArray[1].Title + " " + produktArray[1].Price + " kr.",
                    Margin = new Thickness(5)
                };
                radioPanel.Children.Add(myCheckbox);
                myCheckbox.Checked += CheB_Checked; // TODO: Koordinat (1, 0) ska visa objektets bild. Plus att objektet ska in i varukorgen?
                myCheckbox.Unchecked += CheB_Unchecked; // Bilden ska försvinna. + bort fr varukorg.
            }

            // LÄGGA TILL FLERA BILDER (Spirit (sista objektet) till):
            foreach (string imagePath in imagePaths)
            {
                // "For each file path, create an image using the various classes below and add it to the wrap panel.
                // If we have to do this in many places, we would preferably create a custom method for doing it easily, as in the kitchen sink example."
                ImageSource source = new BitmapImage(new Uri(imagePath, UriKind.Relative));
                Image image = new Image
                {
                    Source = source,
                    Width = 100,
                    Height = 100,
                    Stretch = Stretch.UniformToFill,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(5)
                };
                RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);
                grid.Children.Add(image);
                Grid.SetRow(image, 1); // ändra sedan
                Grid.SetColumn(image, 0); //
            }



            // Lägga till bara EN bild (funkar):
            string myImagePath = "Images/violator.jpg"; // Måste stå Images först
            //string path;
            //for (int i = 0; i < imagePaths.Length; i++)
            //{
            //    path = imagePaths[i];
            //}
            ImageSource mySource = new BitmapImage(new Uri(myImagePath, UriKind.Relative));
            Image myImage = new Image
            {
                Source = mySource,
                Width = 100,
                Height = 100,
                Stretch = Stretch.UniformToFill,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5)
            };
            RenderOptions.SetBitmapScalingMode(myImage, BitmapScalingMode.HighQuality);
            albumInfoGrid.Children.Add(myImage);
            Grid.SetRow(myImage, 1);
            Grid.SetColumn(myImage, 0);





            myTextblock = new TextBlock { Text = "Varuinfo/kvitto/annan info" };
            grid.Children.Add(myTextblock);
            Grid.SetRow(myTextblock, 2);
            Grid.SetColumn(myTextblock, 0);
            Grid.SetColumnSpan(myTextblock, 3);
        }




        // ======================================================
        // ============= METODER ================================
        // ======================================================




        private void AmountCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 
            userAmountChoice = 0;
            for (int i = 0; i < amountCombobox.Items.Count; i++)
            {
                //if (amountCombobox.Items.Contains("1")  // kolla intellisense!
                //{

                //}
            }
        }

        private void CheB_Unchecked(object sender, RoutedEventArgs e)
        {
            // uppdatera Cart.csv?
        }

        private void CheB_Checked(object sender, RoutedEventArgs e)
        {
            // aktuell bild ska visas i (0, 1)

        }

        private void ComboboxVald(object sender, SelectionChangedEventArgs e)
        {
            // "At this point, we know that the selection in the combobox has changed but we don't know how.
            // First, we figure out the index of the newly selected value in the combobox."
            int index = cb.SelectedIndex;

            // "We then look up the corresponding person in the list of people."
            DepecheModeAlbum d = albumList[index];
            //DepecheModeAlbum dd = produktArray[index];
            
            // ovan index ska in i dictionary:
            for (int i = 0; i < albumList.Count; i++)
            {
                Cart = new Dictionary<DepecheModeAlbum, int>
                {
                    [albumList[i]] = 1 // eller produktArray 
                };
                //Cart.Add(dd, 1); // kraschar, an item with the same key has already been added
                //Cart.Add(d, 1);
            }

            //Cart.Add(DepecheModeAlbum, int); <- formeln




            // Skriva dictionary-informationen till Cart.csv:
            // "Create an empty list of text lines that we will fill with strings and then write to a textfile using `WriteAllLines`."
            List<string> linesCart = new List<string>();
            foreach (KeyValuePair <DepecheModeAlbum, int> pair in Cart)
            {
                DepecheModeAlbum a = pair.Key;
                int amount = pair.Value;
                // "For each product, we only save the code and the amount.
                // The other info (name, price, description) is already in "Products.csv" and we can look it up when we load the cart."
                linesCart.Add(a.Title + "," + amount);
            }
            File.WriteAllLines(CartPath, linesCart); // Fungerar(?)

            
            // Skriva ut bild till användaren här?


            // "We finally update the label with that person's info."
            myTextblock.Text = d.Title + " för " + d.Price + "kr har du valt (men inte köpt)."; // funkar

            // ska jag uppdatera cart här också? dvs Buy också
        }

        private void Buy(object sender, RoutedEventArgs e)
        {
            Cart.Clear(); // Null Ref Exc
            File.Delete(CartPath);
            myTextblock.Text = "Du har köpt " + cb.SelectedItem; // funkar

            sum = 0;
            // summa = objekt.Price * amount;
            // + beräkna lite andra grejer också
        }

        private void TaBortAllt(object sender, RoutedEventArgs e)
        {
            // Rensa dictionary:
            Cart.Clear(); // Null Ref Exc
            // Rensa csv (om inte kunden vill spara den till senare!):
            File.Delete(CartPath);
            // Ge användaren information:
            myTextblock.Text = "Varukorgen är tom nu!"; // funkar
        }

        private void TaBortVara(object sender, RoutedEventArgs e)
        {
            // sätta value till 0 för aktuell key?
            // lättast om de får skriva in antal de vill ta bort? Och checkbox för titel... felhantering input då.
        }




        // TESTER
        // Läsa från Products.csv
        // Skriva till Cart.csv
        // Läsa från Cart.csv
        // Beräkning av summa inkl. rabatt
        // Beräkning av rabatt
        // Beräkning av total rabatt
    }
}
