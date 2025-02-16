using System;
using System.Collections.Generic;

namespace Blackjack
{
    // Klasa reprezentująca kartę
    public class Karta
    {
        public string Nazwa { get; set; } // Nazwa karty (np. 2, 3, Walet, Dama, Król, As)
        public string Kolor { get; set; } // Kolor karty (np. Kier, Karo, Trefl, Pik)
        public int Wartosc { get; set; } // Wartość karty (np. 2, 3, 10, 11)

        // Konstruktor klasy Karta
        public Karta(string nazwa, string kolor, int wartosc)
        {
            Nazwa = nazwa;
            Kolor = kolor;
            Wartosc = wartosc;
        }

        // Metoda zwracająca ASCII art karty
        public string[] AsciiArt()
        {
            string[] asciiKarta = new string[5];
            string nazwaKarty = Nazwa.Length == 1 ? Nazwa + " " : Nazwa; // Dodaj spację dla jednocyfrowych kart

            asciiKarta[0] = "┌─────────┐";
            asciiKarta[1] = $"│{nazwaKarty}       │";
            asciiKarta[2] = "│         │";
            asciiKarta[3] = $"│    {Kolor.Substring(0, 1)}    │"; // Używamy pierwszej litery koloru
            asciiKarta[4] = "└─────────┘";

            return asciiKarta;
        }

        // Nadpisanie metody ToString() dla klasy Karta
        public override string ToString()
        {
            return $"{Nazwa} {Kolor}";
        }
    }

    // Klasa reprezentująca talię kart
    public class Talia
    {
        public List<Karta> Karty { get; set; } // Lista kart w talii

        // Konstruktor klasy Talia
        public Talia()
        {
            Karty = new List<Karta>();
            string[] kolory = { "Kier", "Karo", "Trefl", "Pik" };
            string[] nazwy = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "Walet", "Dama", "Król", "As" };

            // Tworzenie kart i dodawanie ich do talii
            foreach (var kolor in kolory)
            {
                for (int i = 0; i < nazwy.Length; i++)
                {
                    int wartosc = i + 2;
                    if (i >= 9) wartosc = 10; // Walet, Dama, Król mają wartość 10
                    if (i == 12) wartosc = 11; // As ma wartość 11
                    Karty.Add(new Karta(nazwy[i], kolor, wartosc));
                }
            }
        }

        // Metoda tasująca talię kart
        public void Tasuj()
        {
            Random rand = new Random();
            for (int i = Karty.Count - 1; i > 0; i--)
            {
                int j = rand.Next(0, i + 1);
                Karta temp = Karty[i];
                Karty[i] = Karty[j];
                Karty[j] = temp;
            }
        }

        // Metoda dobierająca kartę z talii
        public Karta DobierzKarte()
        {
            Karta karta = Karty[0];
            Karty.RemoveAt(0);
            return karta;
        }
    }

    // Klasa reprezentująca gracza
    public class Gracz
    {
        public List<Karta> Reka { get; set; } // Lista kart w ręce gracza
        public int Pieniadze { get; set; } // Ilość pieniędzy gracza
        public int Zaklad { get; set; } // Aktualny zakład gracza

        // Konstruktor klasy Gracz
        public Gracz(int pieniadze = 100)
        {
            Reka = new List<Karta>();
            Pieniadze = pieniadze;
            Zaklad = 0;
        }

        // Metoda dodająca kartę do ręki gracza
        public void DobierzKarte(Karta karta)
        {
            Reka.Add(karta);
        }

        // Metoda obliczająca sumę punktów w ręce gracza
        public int ObliczPunkty()
        {
            int suma = 0;
            int liczbaAsow = 0;

            foreach (var karta in Reka)
            {
                if (karta.Nazwa == "As")
                {
                    liczbaAsow++;
                    suma += 11;
                }
                else
                {
                    suma += karta.Wartosc;
                }
            }

            // Jeśli suma punktów przekracza 21, zmniejsz wartość asów z 11 na 1
            while (suma > 21 && liczbaAsow > 0)
            {
                suma -= 10;
                liczbaAsow--;
            }

            return suma;
        }

        // Metoda wyświetlająca karty w ręce gracza
        public void WyswietlReke()
        {
            string[] linie = new string[5];

            foreach (var karta in Reka)
            {
                string[] asciiKarta = karta.AsciiArt();
                for (int i = 0; i < 5; i++)
                {
                    linie[i] += asciiKarta[i] + " ";
                }
            }

            foreach (var linia in linie)
            {
                Console.WriteLine(linia);
            }

            Console.WriteLine($"Suma punktów: {ObliczPunkty()}");
        }

        // Metoda ustawiająca zakład gracza
        public void PostawZaklad(int zaklad)
        {
            if (zaklad > Pieniadze)
            {
                Console.WriteLine("Nie masz wystarczająco pieniędzy!");
                return;
            }
            Zaklad = zaklad;
            Pieniadze -= zaklad;
        }

        // Metoda wywoływana w przypadku wygranej gracza
        public void Wygrana()
        {
            Pieniadze += Zaklad * 2;
            Zaklad = 0;
        }

        // Metoda wywoływana w przypadku remisu
        public void Remis()
        {
            Pieniadze += Zaklad;
            Zaklad = 0;
        }
    }

    // Główna klasa gry
    public class Blackjack
    {
        public Talia Talia { get; set; } // Talia kart
        public Gracz Gracz { get; set; } // Gracz
        public Gracz Krupier { get; set; } // Krupier

        // Konstruktor klasy Blackjack
        public Blackjack()
        {
            Talia = new Talia();
            Talia.Tasuj();
            Gracz = new Gracz();
            Krupier = new Gracz();
        }

        // Metoda rozdająca początkowe karty graczowi i krupierowi
        public void RozdajPoczatkoweKarty()
        {
            Gracz.DobierzKarte(Talia.DobierzKarte());
            Krupier.DobierzKarte(Talia.DobierzKarte());
            Gracz.DobierzKarte(Talia.DobierzKarte());
            Krupier.DobierzKarte(Talia.DobierzKarte());
        }

        // Metoda wyświetlająca stan gry
        public void WyswietlStanGry(bool pokazKartyKrupiera = false)
        {
            Console.WriteLine("Twoje karty:");
            Gracz.WyswietlReke();

            Console.WriteLine("\nKarty krupiera:");
            if (pokazKartyKrupiera)
            {
                Krupier.WyswietlReke();
            }
            else
            {
                string[] asciiKarta = Krupier.Reka[0].AsciiArt();
                foreach (var linia in asciiKarta)
                {
                    Console.WriteLine(linia);
                }
                Console.WriteLine("┌─────────┐");
                Console.WriteLine("│░░░░░░░░░│");
                Console.WriteLine("│░░░░░░░░░│");
                Console.WriteLine("│░░░░░░░░░│");
                Console.WriteLine("└─────────┘");
            }
        }

        // Metoda resetująca talię kart
        public void ResetujTalie()
        {
            Talia = new Talia();
            Talia.Tasuj();
        }

        // Metoda resetująca grę
        public void ResetujGre()
        {
            Gracz.Reka.Clear();
            Krupier.Reka.Clear();
            ResetujTalie();
        }

        // Metoda główna gry
        public void Graj()
        {
            ResetujGre(); // Reset gry przed rozpoczęciem nowej rundy

            Console.WriteLine($"Masz {Gracz.Pieniadze} zł. Ile chcesz postawić?");
            int zaklad;
            while (!int.TryParse(Console.ReadLine(), out zaklad) || zaklad <= 0 || zaklad > Gracz.Pieniadze)
            {
                Console.WriteLine("Nieprawidłowy zakład. Podaj poprawną kwotę.");
            }
            Gracz.PostawZaklad(zaklad);

            RozdajPoczatkoweKarty();
            WyswietlStanGry();

            // Pętla gry dla gracza
            while (true)
            {
                Console.WriteLine("\nCo chcesz zrobić? (1 - Dobierz kartę, 2 - Pas)");
                string wybor = Console.ReadLine();

                if (wybor == "1")
                {
                    Gracz.DobierzKarte(Talia.DobierzKarte());
                    WyswietlStanGry();

                    if (Gracz.ObliczPunkty() > 21)
                    {
                        Console.WriteLine("Przekroczyłeś 21 punktów! Przegrywasz.");
                        return;
                    }
                }
                else if (wybor == "2")
                {
                    break;
                }
            }

            // Pętla gry dla krupiera
            while (Krupier.ObliczPunkty() < 17)
            {
                Krupier.DobierzKarte(Talia.DobierzKarte());
            }

            WyswietlStanGry(true);

            // Sprawdzenie wyników gry
            if (Krupier.ObliczPunkty() > 21 || Gracz.ObliczPunkty() > Krupier.ObliczPunkty())
            {
                Console.WriteLine("Gratulacje! Wygrałeś!");
                Gracz.Wygrana();
            }
            else if (Gracz.ObliczPunkty() == Krupier.ObliczPunkty())
            {
                Console.WriteLine("Remis!");
                Gracz.Remis();
            }
            else
            {
                Console.WriteLine("Niestety, przegrałeś.");
            }

            Console.WriteLine($"Twój stan konta: {Gracz.Pieniadze} zł");
        }
    }

    // Główna klasa programu
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Witaj w grze Blackjack!");

            // Inicjalizacja gracza z początkowym balansem
            Gracz gracz = new Gracz(100);
            bool grajDalej = true;

            // Pętla główna programu
            while (grajDalej)
            {
                // Sprawdź, czy gracz ma jeszcze pieniądze
                if (gracz.Pieniadze <= 0)
                {
                    Console.WriteLine("Nie masz już pieniędzy. Koniec gry.");
                    break;
                }

                // Inicjalizacja nowej gry z aktualnym balansem gracza
                Blackjack gra = new Blackjack();
                gra.Gracz = gracz; // Użyj istniejącego gracza z aktualnym balansem
                gra.Graj();

                // Sprawdź ponownie stan konta po zakończeniu gry
                if (gracz.Pieniadze <= 0)
                {
                    Console.WriteLine("Nie masz już pieniędzy. Koniec gry.");
                    break;
                }

                Console.WriteLine("Czy chcesz zagrać ponownie? (t/n)");
                string odpowiedz = Console.ReadLine().ToLower();

                while (odpowiedz != "t" && odpowiedz != "n")
                {
                    Console.WriteLine("Nieprawidłowa odpowiedź. Wpisz 't' aby grać ponownie lub 'n' aby zakończyć.");
                    odpowiedz = Console.ReadLine().ToLower();
                }

                if (odpowiedz == "n")
                {
                    grajDalej = false;
                }
            }

            Console.WriteLine("Dziękujemy za grę!");
        }
    }
}
