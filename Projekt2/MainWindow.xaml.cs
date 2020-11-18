using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
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

namespace Projekt2
{
    public class Album
    {
        public string Title;
        public string Year;
        public decimal Price;
    }
    public partial class MainWindow : Window
    {
        //public static List<Album> albums = new List<Album>(); 
        private ListBox records;
        private TextBox recordInfo;
        private Button AddToCartButton;
        private Button removeFromCartButton;
        private Button removeAllObjectsInCartButton;
        private Button saveCartButton;
        private ListBox cart;
        private Label totalAmountInCart;
        private List<Album> cartList = new List<Album>();
        private Album[] cartArray;
        private Album[] produktArray;
        private List<Album> albumListing = new List<Album>();
        string[] linesProduct = File.ReadAllLines("Album.csv");
        string title, year; decimal price;
        private decimal sum = 0;
        public const string CartPath = @"C:\Windows\Temp\Cart.csv";
        public MainWindow()
        {
            InitializeComponent();
            Start();
        }

        private void Start()
        {

            foreach (string line in linesProduct)
            {
                string[] parts = line.Split(',');
                title = parts[0];
                year = parts[1];
                price = decimal.Parse(parts[2]);

                albumListing.Add(new Album
                {
                    Title = title,
                    Year = year,
                    Price = price,
                   
                });
            }
            produktArray = albumListing.ToArray();

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
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            Grid shopPanel = CreateShopPanel();
            grid.Children.Add(shopPanel);
            Grid.SetRow(shopPanel, 0);
            Grid.SetColumn(shopPanel, 0);

            StackPanel checkoutPanel = CreateCheckoutPanel();
            grid.Children.Add(checkoutPanel);
            Grid.SetRow(checkoutPanel, 0);
            Grid.SetColumn(checkoutPanel, 1);




        }
        private Grid CreateShopPanel()
        {
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });


            TextBlock shopName = new TextBlock
            {
                Text = "The DEPECHE MODE record shop",
                //TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                FontSize = 20,
                TextAlignment = TextAlignment.Center
            };
            grid.Children.Add(shopName);
            Grid.SetColumn(shopName, 0);
            Grid.SetRow(shopName, 0);
            Grid.SetColumnSpan(shopName, 2);

            TextBlock albumList = new TextBlock
            {
                Text = "Album",
                Margin = new Thickness(5),
                FontSize = 15,
                TextAlignment = TextAlignment.Left
            };
            grid.Children.Add(albumList);
            Grid.SetColumn(albumList, 0);
            Grid.SetRow(albumList, 1);


            records = new ListBox { };
            foreach (Album item in albumListing) 
            {
                records.Items.Add(item.Title); 
            }
            
            grid.Children.Add(records);
            Grid.SetColumn(records, 0);
            Grid.SetRow(records, 2);
            records.SelectionChanged += Records_SelectionChanged;
            

            recordInfo = new TextBox
            {
                Margin = new Thickness(5),

            };
            grid.Children.Add(recordInfo);
            Grid.SetColumn(recordInfo, 1);
            Grid.SetRow(recordInfo, 2);



            return grid;
        }

        private void Records_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = records.SelectedIndex;

            Album d = albumListing[index];

            recordInfo.Text = "The album " + d.Title + Environment.NewLine + "was released in " + d.Year
                + Environment.NewLine + "we are selling it for the price: " + d.Price + "kr";
        }

        private StackPanel CreateCheckoutPanel()
        {
            StackPanel checkoutPanel = new StackPanel { Orientation = Orientation.Vertical };

            TextBlock shopName = new TextBlock
            {
                Text = "Checkout",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                FontSize = 20,
                TextAlignment = TextAlignment.Center
            };
            checkoutPanel.Children.Add(shopName);

            AddToCartButton = new Button
            {
                Content = "Add to cart",
                Padding = new Thickness(5),
                Margin = new Thickness(5),
                FontSize = 15,
                    
            };
            checkoutPanel.Children.Add(AddToCartButton);
            AddToCartButton.Click += AddToCartButton_Click;

            removeFromCartButton = new Button
            {
                Content = "Remove from cart",
                Padding = new Thickness(5),
                Margin = new Thickness(5),
                FontSize = 15,
            };
            checkoutPanel.Children.Add(removeFromCartButton);
            removeFromCartButton.Click += RemoveFromCartButton_Click;

            removeAllObjectsInCartButton = new Button
            {
                Content = "Remove all items from cart",
                Padding = new Thickness(5),
                Margin = new Thickness(5),
                FontSize = 15,
            };
            checkoutPanel.Children.Add(removeAllObjectsInCartButton);
            removeAllObjectsInCartButton.Click += RemoveAllObjectsInCartButton_Click;


            cart = new ListBox{};
            checkoutPanel.Children.Add(cart);
           
           

            totalAmountInCart = new Label{};
            totalAmountInCart.Content = "Total " + sum + " Kr";
            checkoutPanel.Children.Add(totalAmountInCart);

            saveCartButton = new Button
            {
                Content = "Save cart",
                Padding = new Thickness(5),
                Margin = new Thickness(5),
                FontSize = 15,
            };
            checkoutPanel.Children.Add(saveCartButton);
            saveCartButton.Click += SaveCartButton_Click;

            return checkoutPanel;
        }

        private void SaveCartButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> linesCart = new List<string>();
            foreach(Album saveCart in cartList)
            {
                Title = saveCart.Title;
                price = saveCart.Price;
                linesCart.Add(saveCart.Title + "," + saveCart.Price);
            }
            File.WriteAllLines(CartPath, linesCart);
            MessageBox.Show("Your cart is saved");

        }

        private void RemoveAllObjectsInCartButton_Click(object sender, RoutedEventArgs e)
        {
            //int index = cart.SelectedIndex;
            //Album d = cartList[index];
            cart.Items.Clear();
            cartList.Clear();
            sum = 0;
            totalAmountInCart.Content = "Total " + sum + " Kr";
        }

        private void RemoveFromCartButton_Click(object sender, RoutedEventArgs e)
        {
            int index = cart.SelectedIndex;
            Album d = cartList[index];
            cart.Items.RemoveAt(index);
            cartList.RemoveAt(index);
            sum -= d.Price;
            totalAmountInCart.Content = "Total " + sum + " Kr";
        }
        private void AddToCartButton_Click(object sender, RoutedEventArgs e)
        {
            int index = records.SelectedIndex;
            Album d = albumListing[index];
           cart.Items.Add("Depeche Mode " + "       Album: " + d.Title + "      Pris: " + d.Price + "Kr");
            sum += d.Price;
            totalAmountInCart.Content = "Total " + sum + " Kr";

            cartList.Add(new Album
            {
                Title = d.Title,
                Price = d.Price
            });
            
        }
    }
}
