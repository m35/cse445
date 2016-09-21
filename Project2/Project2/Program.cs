﻿using System;
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
        static Random roomCount = new Random();

        public static event priceCutEvent promotionalEvent; //price cut event
        private static Int32 currentRoomPrice = 200;

        //Return the room price.
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

        //PRICING MODEL
        public void HotelAdvertiseFunc()
        {
            // Has to take in some sort of variable SH
            for (Int32 i = 0; i < 36; i++)
            {
                Thread.Sleep(2000);
                Int32 currentSeason = season.Next(1, 4);
                Int32 roomsAvailable = roomCount.Next(1, 500);
                Int32 newRoomPrice = roomPrice.Next(50, 500);
                if (currentSeason == 1) //off season
                {
                    if (roomsAvailable > 250)
                    {
                        newRoomPrice = roomPrice.Next(50, 150);
                    }
                    else if (roomsAvailable < 250 && roomsAvailable > 50)
                    {
                        newRoomPrice = roomPrice.Next(150, 250);
                    }
                    else
                    {
                        newRoomPrice = roomPrice.Next(250, 500);
                    }
                    
                }
                else if(currentSeason == 3) //busy season
                {
                    if (roomsAvailable > 250)
                    {
                        newRoomPrice = roomPrice.Next(200, 300);
                    }
                    else if (roomsAvailable < 250 && roomsAvailable > 50)
                    {
                        newRoomPrice = roomPrice.Next(300, 400);
                    }
                    else
                    {
                        newRoomPrice = roomPrice.Next(400, 500);
                    }

                }

                
                Console.WriteLine("-------------------------------------------------------------------New room price is ${0}", newRoomPrice);
                Hotel.changePrice(newRoomPrice);
            }
        }
    }

    public class TravelAgency
    {
        static Random numberOfRooms = new Random();
        public void getHotelRates()
        {
            Hotel randomHotel = new Hotel();
            for (Int32 i = 0; i < 10; i++)
            {
                Thread.Sleep(1000);
                Int32 roomPrice = randomHotel.getPrice();
                Console.WriteLine("Travel Agency{0} has everyday low price: ${1} per room", Thread.CurrentThread.Name, roomPrice);
            }
        }

        //Buy the discount rooms here
        public void discountRooms(Int32 p)
        {
            
            Console.WriteLine("Travel Agency{0} has rooms for sale for as low as ${1} each", Thread.CurrentThread.Name, p);
            OrderObject purchaseOrder = new OrderObject();
            purchaseOrder.amount = numberOfRooms.Next(0, 500);
            purchaseOrder.unitPrice = p;
            //Not sure on these variables source
            purchaseOrder.senderID = Thread.CurrentThread.Name;
            purchaseOrder.receiverID = Thread.CurrentThread.Name;
            purchaseOrder.cardNo = 3;

            Coder.Encode(purchaseOrder);
            
        }
    }

    public class myApplication
    {
        static void Main(string[] args)
        {
            Hotel randomHotel = new Hotel();
            Thread[] hotels = new Thread[3];
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
                travelAgencies[i] = new Thread(new ThreadStart(randomTravelAgency.getHotelRates));
                travelAgencies[i].Name = (i + 1).ToString();
                travelAgencies[i].Start();
            }
        }
    }


    // Multi String Buffer
    //      Fix it if you know it!

    // Need semaphore/write/read for travel agent access
    // can't just read for hotel, but if hotel finds one, it can erase
    public class MultiCellBuffer
    {
        private string[] cell;
        private int cellsInUse = 0;
        private Semaphore _pool;
        private ReaderWriterLock rwLock = new ReaderWriterLock();

        public MultiCellBuffer()
        {
            cell = new string[3];
            _pool = new Semaphore(3, 3);
        }

        public bool checkOpen() { return (cellsInUse < 3); } // true if there's a free cell
        public bool checkEmpty() { return (cellsInUse == 0); } // true if there's nothin there

        // Hotel checking the orders
        public string getCell(string name)
        {
            rwLock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                //_pool.WaitOne();
                // Check each cell for price
                if (checkEmpty()) { }
                else
                {
                    for (int i = 0; i < 3; ++i)
                    {
                        string toChk = cell[i].Split(',')[0];
                        if (String.Compare(name, toChk) == 0)
                        {
                            string tmp = cell[i];
                            cell[i] = "";
                            return tmp;
                        }
                    }

                }
            }
            finally
            {
                rwLock.ReleaseReaderLock();
            }

            return "cbl";  // come back later
            //_pool.Release();
        }

        // Travel agent posting orders
        public int setCell(string order)
        {
            _pool.WaitOne();
            rwLock.AcquireWriterLock(Timeout.Infinite); // not sure what the timeout should be, may be a deadlock
            try
            {
                // Check each cell for price
                if (checkEmpty()) { }
                else
                {
                    cell[cellsInUse] = order;
                    ++cellsInUse;
                }
            }
            finally
            {
                if (cellsInUse > 2)
                    cellsInUse = 2;
                else if (cellsInUse < 0)
                    cellsInUse = 0;

                rwLock.ReleaseWriterLock();
            }

            _pool.Release();
            return 1;
        }
    }

    // Encoder/decoder. Handle non-orders in hotel or travel agent
    // entries seperated by ','
    public static class Coder
    {
        public static string Encode(OrderObject obj)
        {
            string str = obj.receiverID + ',' +
                obj.cardNo + ',' +
                obj.senderID + ',' +
                obj.amount + ',' +
                obj.unitPrice;

            return str;
        }

        public static OrderObject Decode(string str)
        {
            string[] words = str.Split(',');

            // note, receiverID is first since hotels need to read all buffers
            OrderObject obj = new OrderObject();
            obj.receiverID = words[0];
            obj.cardNo = Convert.ToInt32(words[1]);
            obj.senderID = words[2];
            obj.amount = Convert.ToInt32(words[3]);
            obj.unitPrice = Convert.ToDouble(words[4]);

            return obj;
        }
    }
    
    public class OrderObject // receiverID is first just fyi
    {
        public string senderID { get; set; }
        public int cardNo { get; set; }
        public string receiverID { get; set; }
        public int amount { get; set; }
        public double unitPrice { get; set; }
    }
}
