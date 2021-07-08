// Dahil edilen kütüphaneler.
using System; // "Convert", "String" ve "Random" gibi birçok adı kullanabilmemizi sağlayan ad uzayını projemize dahil ettik.
using System.IO; // "FileStream" ve "StreamWriter" gibi birçok adı kullanabilmemizi sağlayan ad uzayını projemize dahil ettik.
using System.Windows; // "MessageBox" gibi bir adı ve butonlarla ilgili birtakım işlemleri yürütebilmemizi sağlayan ad uzayını projemize dahil ettik.
using System.Windows.Media; // Döndürme işlemleri için gerekli "RotateTransform" adını kullanabilmemizi sağlayan ad uzayını projemize dahil ettik.

/// <summary>
/// Ad uzayımızın adını "Carkifelek" olarak belirledik.
/// </summary>
namespace Carkifelek
{
    /// <summary>
    /// MainWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class MainWindow:Window
    {
        /// <summary>
        /// Değişkenler
        /// </summary>
        private readonly Random random = new Random();
        readonly string soru;
        bool jokerVar;
        int puan;
        private readonly string[] gelebilecekSorular = new string[] { "AFYON","AYDIN","BURSA","DÜZCE",
                                                                      "HATAY","İZMİR","IĞDIR","KİLİS",
                /* Örnek sorularımızı buraya yazıyoruz.*/             "KONYA","MUĞLA","NİĞDE","SİİRT",
                                                                      "SİNOP","SİVAS","TOKAT","ÇORUM"};
        
        /// <summary>
        /// Ana penceremizdeki işlemleri yürüten fonksiyonumuz burada yer almaktadır.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent(); // Bileşenleri yürürlüğe koyan fonksiyonumuz burada yer almaktadır.

            // Gelebilecek sorular arasından hangi sorunun seçileceğinin rastgele bir şekilde belirlenmesi.
            soru = gelebilecekSorular[random.Next(gelebilecekSorular.Length)];

            // Soru uzunluğu kadar kutucuk basan döngü.
            for(int i = 0;i<soru.Length;i++)
                Soru.Content=string.Concat(Soru.Content,"[ ]");

            // Soru verimizi dosyaya yazdığımız bölüm burasıdır.
            FileStream fileStream = new FileStream("C:\\Users\\muras\\Desktop\\Carkifelek\\database.txt",FileMode.OpenOrCreate,FileAccess.Write);
            StreamWriter streamWriter = new StreamWriter(fileStream);
            streamWriter.WriteLine(soru);
            streamWriter.Flush();
            streamWriter.Close();
            fileStream.Close();
        }

        /// <summary>
        /// Çarkıfeleği çeviren butonun tıklanma olayı.
        /// </summary>
        /// <param name="sender">Gönderici</param>
        /// <param name="e">Argümanlar</param>
        private void Button_Click(object sender,RoutedEventArgs e)
        {
            RastgeleCevir(); // Çarkıfeleğin dönmesini sağlayan fonksiyonumuzu çağrıyoruz.

            Harf.Content="Bir harf seçiniz.";
            
            switch(GelenPuan.Content) // PAS, İFLAS ve JOKER gelme durumlarına göre işlemler yaptığımız blok.
            {
                case "PAS":
                    Harf.Content="Tekrar çeviriniz.";
                    break;
                case "İFLAS":
                    if(jokerVar)
                    {
                        jokerVar=false;
                        Harf.Content="Jokeriniz var. İflas yok.";
                    }
                    else
                    {
                        puan=0;
                        Puan.Content="Puanınız: 0";
                        Harf.Content="Jokeriniz yok. İflas ettiniz.";
                    }
                    break;
                case "JOKER":
                    jokerVar=true;
                    Harf.Content="Joker kazandınız. İflas yok.";
                    break;
                default:
                    HarfButonDurumlari(true);
                    break;
            }
        }

        /// <summary>
        /// İlgili harfi çeşitli işlemlerden geçirip programın görsel işlemlerini yapan fonksiyonumuz.
        /// </summary>
        /// <param name="harf">İşlemden geçecek harfimiz.</param>
        private void HarfAra(char harf)
        {
            int kacTaneVar = 0;
            string temp;
            if(soru.Contains(harf.ToString())) // Sorumuzun içinde aranan harf var mı?
            {
                for(int i = 0;i<soru.Length;i++) // "Varsa nerelerde?" döngüsü.
                    if(harf==soru[i]) // Harf bulununca bu koşul sağlanır.
                    {
                        temp=string.Empty;
                        for(int j = 0;j<i*3;j++) // Harfin öncesini geçici bir string'e toplayan döngümüz.
                            temp=string.Concat(temp,Soru.Content.ToString()[j]);
                        temp=string.Concat(temp,"[",harf,"]"); // Harfin kendisini kutucuğa ekleyen kod.
                        for(int j = i*3+3;j<soru.Length*3;j++) // Harfin sonrasını geçici bir string'e toplayan döngümüz.
                            temp=string.Concat(temp,Soru.Content.ToString()[j]);
                        Soru.Content=temp;
                        kacTaneVar++; // Aynı harften kaç tane olduğu burada tutuluyor.
                    }

                puan+=Convert.ToInt32(GelenPuan.Content)*kacTaneVar;
                Harf.Content=String.Concat(kacTaneVar,"x",harf," Tebrikler, +",puan," Puan!");
                Puan.Content=string.Concat("Puanınız: ",puan);
            }
            else
                Harf.Content=string.Concat("'",harf,"' harfi yok.");

            HarfButonDurumlari(false); // Harfleri kapamamızı sağlayan fonksiyonumuz.

            if(!Soru.Content.ToString().Contains("[ ]")) // Bulunacak harf kalmadıysa bu sorgu çalışır. Ekrana mesaj kutusu çıkarır.
                MessageBox.Show(string.Concat("Tebrikler!\n",soru," şehrini buldunuz.\nPuanınız: ",puan),
                                "Çarkıfelek",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                
        }

        /// <summary>
        /// Çarkı rastgele çevirmemizi sağlayan fonksiyonumuz.
        /// </summary>
        private void RastgeleCevir()
        {
            // Resmimizi çevirmek için 360'a kadar rastgele bir sayı tuttuğumuz yer.
            int cevirmeAcisi = random.Next(360);

            // Resmimizi rastgele sayımıza göre çevirdiğimiz yer.
            RotateTransform rotateTransform = new RotateTransform(cevirmeAcisi);
            Carkifelek.RenderTransform=rotateTransform;

            // Çarkıfeleğin açılarına göre gelecek değerlerin kodları burada yer almaktadır.
            if(cevirmeAcisi<15)
                GelenPuan.Content=4000;
            else if(cevirmeAcisi<30)
                GelenPuan.Content=1000;
            else if(cevirmeAcisi<45)
                GelenPuan.Content=2000;
            else if(cevirmeAcisi<60)
                GelenPuan.Content="PAS";
            else if(cevirmeAcisi<75)
                GelenPuan.Content=1500;
            else if(cevirmeAcisi<90)
                GelenPuan.Content=2000;
            else if(cevirmeAcisi<105)
                GelenPuan.Content=5000;
            else if(cevirmeAcisi<120)
                GelenPuan.Content=3000;
            else if(cevirmeAcisi<135)
                GelenPuan.Content=1500;
            else if(cevirmeAcisi<150)
                GelenPuan.Content="JOKER";
            else if(cevirmeAcisi<165)
                GelenPuan.Content=2500;
            else if(cevirmeAcisi<180)
                GelenPuan.Content=3000;
            else if(cevirmeAcisi<195)
                GelenPuan.Content=1000;
            else if(cevirmeAcisi<210)
                GelenPuan.Content=3500;
            else if(cevirmeAcisi<225)
                GelenPuan.Content=4000;
            else if(cevirmeAcisi<240)
                GelenPuan.Content="PAS";
            else if(cevirmeAcisi<255)
                GelenPuan.Content=7000;
            else if(cevirmeAcisi<270)
                GelenPuan.Content=4000;
            else if(cevirmeAcisi<285)
                GelenPuan.Content=1000;
            else if(cevirmeAcisi<300)
                GelenPuan.Content=2000;
            else if(cevirmeAcisi<315)
                GelenPuan.Content=1000;
            else if(cevirmeAcisi<330)
                GelenPuan.Content="İFLAS";
            else if(cevirmeAcisi<345)
                GelenPuan.Content=1000;
            else
                GelenPuan.Content=10000;
        }

        /// <summary>
        /// Butonların aktifliğini kontrol eden fonksiyonumuz.
        /// </summary>
        /// <param name="durum">Aktiflik değeri.</param>
        private void HarfButonDurumlari(bool durum)
        {
            A.IsEnabled=durum;
            B.IsEnabled=durum;
            C.IsEnabled=durum;
            Ç.IsEnabled=durum;
            D.IsEnabled=durum;
            E.IsEnabled=durum;
            F.IsEnabled=durum;
            G.IsEnabled=durum;
            Ğ.IsEnabled=durum;
            H.IsEnabled=durum;
            I.IsEnabled=durum;
            İ.IsEnabled=durum;
            J.IsEnabled=durum;
            K.IsEnabled=durum;
            L.IsEnabled=durum;
            M.IsEnabled=durum;
            N.IsEnabled=durum;
            O.IsEnabled=durum;
            Ö.IsEnabled=durum;
            P.IsEnabled=durum;
            R.IsEnabled=durum;
            S.IsEnabled=durum;
            Ş.IsEnabled=durum;
            T.IsEnabled=durum;
            U.IsEnabled=durum;
            Ü.IsEnabled=durum;
            V.IsEnabled=durum;
            Y.IsEnabled=durum;
            Z.IsEnabled=durum;

            Ceviriniz.IsEnabled=!durum;
        }

        private void ClickA(object sender,RoutedEventArgs e) => HarfAra('A'); // "HarfAra" fonksiyonunu ilgili biçimde çağıran A butonunun tıklama olayı.
        private void ClickB(object sender,RoutedEventArgs e) => HarfAra('B'); // "HarfAra" fonksiyonunu ilgili biçimde çağıran B butonunun tıklama olayı.
        private void ClickC(object sender,RoutedEventArgs e) => HarfAra('C'); // "HarfAra" fonksiyonunu ilgili biçimde çağıran C butonunun tıklama olayı.
        private void ClickÇ(object sender,RoutedEventArgs e) => HarfAra('Ç'); // "HarfAra" fonksiyonunu ilgili biçimde çağıran Ç butonunun tıklama olayı.
        private void ClickD(object sender,RoutedEventArgs e) => HarfAra('D'); // "HarfAra" fonksiyonunu ilgili biçimde çağıran D butonunun tıklama olayı.
        private void ClickE(object sender,RoutedEventArgs e) => HarfAra('E'); // "HarfAra" fonksiyonunu ilgili biçimde çağıran E butonunun tıklama olayı.
        private void ClickF(object sender,RoutedEventArgs e) => HarfAra('F'); // "HarfAra" fonksiyonunu ilgili biçimde çağıran F butonunun tıklama olayı.
        private void ClickG(object sender,RoutedEventArgs e) => HarfAra('G'); // "HarfAra" fonksiyonunu ilgili biçimde çağıran G butonunun tıklama olayı.
        private void ClickĞ(object sender,RoutedEventArgs e) => HarfAra('Ğ'); // "HarfAra" fonksiyonunu ilgili biçimde çağıran Ğ butonunun tıklama olayı.
        private void ClickH(object sender,RoutedEventArgs e) => HarfAra('H'); // "HarfAra" fonksiyonunu ilgili biçimde çağıran H butonunun tıklama olayı.
        private void ClickI(object sender,RoutedEventArgs e) => HarfAra('I'); // "HarfAra" fonksiyonunu ilgili biçimde çağıran I butonunun tıklama olayı.
        private void Clickİ(object sender,RoutedEventArgs e) => HarfAra('İ'); // "HarfAra" fonksiyonunu ilgili biçimde çağıran İ butonunun tıklama olayı.
        private void ClickJ(object sender,RoutedEventArgs e) => HarfAra('J'); // "HarfAra" fonksiyonunu ilgili biçimde çağıran J butonunun tıklama olayı.
        private void ClickK(object sender,RoutedEventArgs e) => HarfAra('K'); // "HarfAra" fonksiyonunu ilgili biçimde çağıran K butonunun tıklama olayı.
        private void ClickL(object sender,RoutedEventArgs e) => HarfAra('L'); // "HarfAra" fonksiyonunu ilgili biçimde çağıran L butonunun tıklama olayı.
        private void ClickM(object sender,RoutedEventArgs e) => HarfAra('M'); // "HarfAra" fonksiyonunu ilgili biçimde çağıran M butonunun tıklama olayı.
        private void ClickN(object sender,RoutedEventArgs e) => HarfAra('N'); // "HarfAra" fonksiyonunu ilgili biçimde çağıran N butonunun tıklama olayı.
        private void ClickO(object sender,RoutedEventArgs e) => HarfAra('O'); // "HarfAra" fonksiyonunu ilgili biçimde çağıran O butonunun tıklama olayı.
        private void ClickÖ(object sender,RoutedEventArgs e) => HarfAra('Ö'); // "HarfAra" fonksiyonunu ilgili biçimde çağıran Ö butonunun tıklama olayı.
        private void ClickP(object sender,RoutedEventArgs e) => HarfAra('P'); // "HarfAra" fonksiyonunu ilgili biçimde çağıran P butonunun tıklama olayı.
        private void ClickR(object sender,RoutedEventArgs e) => HarfAra('R'); // "HarfAra" fonksiyonunu ilgili biçimde çağıran R butonunun tıklama olayı.
        private void ClickS(object sender,RoutedEventArgs e) => HarfAra('S'); // "HarfAra" fonksiyonunu ilgili biçimde çağıran S butonunun tıklama olayı.
        private void ClickŞ(object sender,RoutedEventArgs e) => HarfAra('Ş'); // "HarfAra" fonksiyonunu ilgili biçimde çağıran Ş butonunun tıklama olayı.
        private void ClickT(object sender,RoutedEventArgs e) => HarfAra('T'); // "HarfAra" fonksiyonunu ilgili biçimde çağıran T butonunun tıklama olayı.
        private void ClickU(object sender,RoutedEventArgs e) => HarfAra('U'); // "HarfAra" fonksiyonunu ilgili biçimde çağıran U butonunun tıklama olayı.
        private void ClickÜ(object sender,RoutedEventArgs e) => HarfAra('Ü'); // "HarfAra" fonksiyonunu ilgili biçimde çağıran Ü butonunun tıklama olayı.
        private void ClickV(object sender,RoutedEventArgs e) => HarfAra('V'); // "HarfAra" fonksiyonunu ilgili biçimde çağıran V butonunun tıklama olayı.
        private void ClickY(object sender,RoutedEventArgs e) => HarfAra('Y'); // "HarfAra" fonksiyonunu ilgili biçimde çağıran Y butonunun tıklama olayı.
        private void ClickZ(object sender,RoutedEventArgs e) => HarfAra('Z'); // "HarfAra" fonksiyonunu ilgili biçimde çağıran Z butonunun tıklama olayı.
    }
}
