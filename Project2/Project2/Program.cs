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
        Random roomPrice = new Random(); 
        Random season = new Random();
        Random roomCount = new Random();

        public event priceCutEvent promotionalEvent; //price cut event
        private Int32 currentRoomPrice = 200;

        /// <returns>Return the current room price (subject to change without notice, terms and conditions may apply).</returns>
        public Int32 getPrice()
        {
            return currentRoomPrice;
        }

        
        /// <summary>
        /// Changes the price and notifies all listeners (in our case TravelAgencies).
        /// </summary>
        public void changePrice(Int32 currentPrice)
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

        public void receiveOrder(string encodedOrder)
        {
            OrderObject order = Coder.Decode(encodedOrder);

        }

        /// <summary>
        /// OrderProcessing
        /// </summary>
        /// <remarks>
        /// A class or a method in a class on the server's side.
        ///
        /// Whenever an order needs to be processed, a new thread is instantiated from this method to
        /// process the order. 
        /// 
        /// It will check the validity of the credit card number.
        ///
        /// For one/two-member group, you can define your credit card format (example, the credit
        /// card number from the travel agencies must be a number registered to the Hotel, or a
        /// number between two given numbers like 5000 and 7000).
        ///
        /// For the three-member group project, a bank service must be created. Each OrderProcessing
        /// thread will calculate the total amount of charge (e.g., unitPrice*NoOfRooms + Tax + LocationCharge).
        ///
        /// A confirmation must be sent back to the TravelAgency when an order is completed.
        ///
        /// You can implement the confirmation in different ways. For example, you can use another
        /// buffer for the confirmation, where you can use a buffer cell for each thread, so that you
        /// do not have to consider the conflict among the threads. However, you still need to
        /// coordinate the write and read between the producer and the consumer.
        /// </remarks>
        private void orderProcessing()
        {
            Thread processingThread; // = new Thread(?);
            // ... I don't understand what to do next.
            
        }

        /// <summary>PricingModel</summary>
        /// <remarks>
        /// It can be a class or a method in the Hotel class.
        /// 
        /// It decides the price of rooms, which must be between 50 and 500.
        /// It can increase or decrease the price.
        ///
        /// Must be a complex mathematical model where the price must be a function with
        /// multiple parameters (such as the number of orders received within a given time period and
        /// the number of rooms available in the Hotel in the same time period).
        ///
        /// In other words, the function must take the amount of orders as input.
        ///
        /// May use a hard-coded table of the price in each weekday.
        ///
        /// The model must allow the price to go up some times and go down
        /// other times within your iterations of testing.
        /// </remarks>
        /// <returns>The new price for a room.</returns>
        private Int32 pricingModel() // "the function must take the amount of orders as input"
        {
            Int32 newRoomPrice;
            Int32 currentSeason = season.Next(1, 4);
            Int32 roomsAvailable = roomCount.Next(1, 500);
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
            else if (currentSeason == 3) //busy season
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
            else
            {
                newRoomPrice = roomPrice.Next(50, 500);
            }

            return newRoomPrice;
        }

        /// <summary>Entry point for Hotel thread.</summary>
        public void HotelAdvertiseFunc()
        {
            const Int32 PRICE_CHANGES_UNTIL_EXIT = 36;

            for (Int32 i = 0; i < PRICE_CHANGES_UNTIL_EXIT; i++)
            {
                Thread.Sleep(2000);

                // Has to take in some sort of variable SH
                Int32 newRoomPrice = pricingModel(); // "the function must take the amount of orders as input"
                Console.WriteLine("-------------------------------------------------------------------New room price is ${0}", newRoomPrice);
                changePrice(newRoomPrice);
            }
        }
    }

    public class TravelAgency
    {
        Random numberOfRooms = new Random();
        /// <summary>Entry point for TravelAgency thread.</summary>
        /// <remarks>
        /// The thread will terminate after the Hotel thread has terminated.
        /// </remarks>
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

        /// <summary>Buy the discount rooms here</summary>
        /// <remarks>
        /// Called by Hotel when a price cut occurs.
        /// 
        /// Each travel agency contains a call-back method (event handler) for the Hotel to call when
        /// a price-cut event occurs.
        ///
        /// The travel agency will calculate the number of rooms to order, for example, based on the
        /// need and the difference between the previous price and the current price.
        /// </remarks>
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

        /* Not sure where this is supposed to happen
        Each order is an OrderClass object. 
        The object is sent to the Encoder for encoding. 
        The encoded string is sent back to the travel agency. 
        Then, the travel agency will send the order in string format to the MultiCellBuffer. 
        Before sending the order to the MultiCellBuffer, a time stamp must be saved. 
        When the confirmation of order completion is received, the time of the order will be calculated and saved or printed.
        */
    }

    public class MainProgram
    {
        static void Main(string[] args)
        {
            Hotel[] hotels = new Hotel[3];
            Thread[] hotelThreads = new Thread[3];
            for (int i = 0; i < 3; i++)
            {
                hotels[i] = new Hotel();
                hotelThreads[i] = new Thread(new ThreadStart(hotels[i].HotelAdvertiseFunc));
                hotelThreads[i].Name = String.Format("Hotel {0} thread", i + 1);
                hotelThreads[i].Start();
            }

            Thread[] travelAgencyThreads = new Thread[5];
            for (int i = 0; i < 5; i++)
            {
                TravelAgency aTravelAgency = new TravelAgency();
                for (int j = 0; j < hotels.Length; i++)
                {
                    hotels[j].promotionalEvent += new priceCutEvent(aTravelAgency.discountRooms);
                }
                travelAgencyThreads[i] = new Thread(new ThreadStart(aTravelAgency.getHotelRates));
                travelAgencyThreads[i].Name = (i + 1).ToString();
                travelAgencyThreads[i].Start();
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
            // wait forever until other threads are done with their work 
            // (should never deadlock unless another thread never leaves)
            rwLock.AcquireWriterLock(Timeout.Infinite); 
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
                --cellsInUse;
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

    // Bank service; holds the amounts in the bank account and decrypts credit card no's
    public class BankService
    {
        private int[] accountAmount;
        private int[] cardNumber;

        public BankService()
        {
            accountAmount = new int[5];
            cardNumber = new int[5];
        }

        public string chargeAccount(string cardNo, int amount) // card is encrypted
        {
            Project2.EncryptSvc.ServiceClient client = new Project2.EncryptSvc.ServiceClient();
            int cnum = Convert.ToInt32(client.Decrypt(cardNo));

            for(int i = 0; i < 5; ++i)
            {
                if(cnum == cardNumber[i] &&
                    amount < accountAmount[i])
                {
                    accountAmount[i] -= amount;
                    return "valid";
                }
            }
            return "not valid";
        }

        public int cardApplication(int amount) // outs the card num
        {
            int i = 0;
            while (cardNumber[i] == 0)
                ++i;

            Random rand = new Random();
            int temp = (i + 1) * 1000 + rand.Next(0, 999);

            cardNumber[i] = temp;
            accountAmount[i] = amount;

            return cardNumber[i];
        }

        // just part of the required parts of bank
        public int deposit(int cardNo, int amount) // Don't have to use
        {
            for (int i = 0; i < 5; ++i)
            {
                if (cardNo == cardNumber[i])
                {
                    accountAmount[i] += amount;
                    return 0; // success
                }
            }

            return -1; // card not found
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
                obj.unitPrice + ',' +
                obj.timestamp;

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
            obj.timestamp = Convert.ToInt32(words[5]);

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
        public int timestamp { get; set; }
    }
}
