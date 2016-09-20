using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace HotelBookingSystem
{
    public delegate void priceCutEvent(Int32 price);
    public class Hotel
    {
        static Random roomPrice = new Random(); 
        static Random season = new Random(); 
        public static event priceCutEvent promotionalEvent; //price cut event
        private static Int32 currentRoomPrice = 200;


        public Int32 getPrice()
        {
            return currentRoomPrice;
        }

        
        public static void changePrice(Int32 currentPrice)
        {
            if (currentPrice < currentRoomPrice)
            {
                if (promotionalEvent != null)
                {
                    promotionalEvent(currentPrice);
                }
            }
            currentRoomPrice = currentPrice;
        }

        //Every 2 seconds release a new room price based on the season
        public void HotelAdvertiseFunc()
        {
            for (Int32 i = 0; i < 36; i++)
            {
                Thread.Sleep(2000);
                Int32 currentSeason = season.Next(1, 4);
                Int32 newRoomPrice = roomPrice.Next(50, 300);
                if (currentSeason == 1) //off season
                {
                    newRoomPrice = roomPrice.Next(50, 100);
                    //Console.WriteLine("----------------Off Season--------------------------");
                }
                else if(currentSeason == 3) //busy season
                {
                    newRoomPrice = roomPrice.Next(50, 600);
                    //Console.WriteLine("----------------Busy Season--------------------------");
                }
                 
                
                
                
                Console.WriteLine("-------------------------------------------------------------------New room price is ${0}", newRoomPrice);
                Hotel.changePrice(newRoomPrice);
            }
        }
    }

    public class TravelAgency
    {
        public void buyTheRooms()
        {
            Hotel randomHotel = new Hotel();
            for (Int32 i = 0; i < 10; i++)
            {
                Thread.Sleep(1000);
                Int32 roomPrice = randomHotel.getPrice();
                Console.WriteLine("Travel Agency{0} has everyday low price: ${1} per room", Thread.CurrentThread.Name, roomPrice);
            }
        }


        public void discountRooms(Int32 p)
        {
            Console.WriteLine("Travel Agency{0} has rooms for sale for as low as ${1} each", Thread.CurrentThread.Name, p);
        }
    }


    public class myApplication
    {
        static void Main(string[] args)
        {
            Hotel randomHotel = new Hotel();
            Thread[] hotels = new Thread[3];//
            for (int i = 0; i < 3; i++)
            {
                hotels[i] = new Thread(new ThreadStart(randomHotel.HotelAdvertiseFunc));
                hotels[i].Name = (i + 1).ToString();
                hotels[i].Start();
            }

            TravelAgency randomTravelAgency = new TravelAgency();
            Hotel.promotionalEvent += new priceCutEvent(randomTravelAgency.discountRooms);
            Thread[] travelAgencies = new Thread[5];
            for (int i = 0; i < 5; i++)
            {
                travelAgencies[i] = new Thread(new ThreadStart(randomTravelAgency.buyTheRooms));
                travelAgencies[i].Name = (i + 1).ToString();
                travelAgencies[i].Start();
            }
        }
    }




}
