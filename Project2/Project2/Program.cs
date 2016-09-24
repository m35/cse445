using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace HotelBookingSystem
{
    public delegate void priceCutEvent(string hotelName, Int32 oldPrice, Int32 newPrice);
    
   
    public class Hotel
    {
        private Random roomPrice = new Random();
        private Random season = new Random();
        private Random roomCount = new Random();
        private const double tax = .08;
        private const double locCharge = 10;

        public event priceCutEvent promotionalEvent; //price cut event
        private Int32 currentRoomPrice = 200;

        /// <summary>
        /// There is a counter p in the Hotel. After p (e.g., p = 20) price cuts have been made, a
        /// Hotel thread will terminate.
        /// </summary>
        private int priceCutsUntilExit = 36;

        private string hotelName;
        public string Name { get { return hotelName; } }

        public Hotel(string name)
        {
            hotelName = name;
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
                    promotionalEvent(hotelName, currentRoomPrice, currentPrice);
                }
                Console.WriteLine("[Hotel {0}] {1} price cuts remaining", Name, priceCutsUntilExit);
                priceCutsUntilExit--;
            }
            currentRoomPrice = currentPrice;
        }

        ///<summary>
        ///Attempt to receive the order. If orders are available then decode.
        ///</summary> 
        public void receiveOrder()
        {
            // check if any orders are available
            string encodedOrder = MultiCellBuffer.agency2hotel.getCell(Name);
            if (encodedOrder != MultiCellBuffer.COME_BACK_LATER)
            {
                OrderObject order = Coder.Decode(encodedOrder);
                // process the received order
                orderProcessing(order);
            }
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
        private void orderProcessing(OrderObject obj)
        {
            string validation, msg;
            double toCharge = (obj.unitPrice * Convert.ToDouble(obj.amount)) * (1.0 + tax) + locCharge;

            DateTime now = DateTime.Now;
            string nowString = now.ToShortDateString() + " " + now.ToShortTimeString();

            validation = BankService.centralBank.chargeAccount(encryptCC(obj.cardNo), toCharge);
            msg = String.Format("[Hotel {0}] Order for agency {1} started at {2}, {3} at {4}",
                                Name, obj.senderID, obj.timestamp, validation, nowString);
            MultiCellBuffer.hotel2agency.setCell(obj.senderID, msg);
        }

        /// <summary>
        /// Encrypt a card number.
        /// </summary>
        /// <param name="cardNo"></param>
        /// <returns>Encrypted card number.</returns>
        public static string encryptCC(int cardNo)
        {
            Project2.EncryptSvc.ServiceClient client = new Project2.EncryptSvc.ServiceClient();

            return client.Encrypt(Convert.ToString(cardNo));
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
            while (priceCutsUntilExit >= 0)
            {
                Thread.Sleep(2000);

                // Has to take in some sort of variable SH
                Int32 newRoomPrice = pricingModel(); // "the function must take the amount of orders as input"
                Console.WriteLine("[Hotel {0}] New room price is ${1}", Name, newRoomPrice);
                changePrice(newRoomPrice);
                receiveOrder();
            }
        }
    }

    public class BufferCell
    {
        public BufferCell(string r, string v)
        {
            receiver = r;
            value = v;
        }
        public string receiver { get; set; }
        public string value { get; set; }
    }

    public class TravelAgency
    {
        private string agencyName;
        public string Name { get { return agencyName; } }

        private Random numberOfRooms = new Random();
        private Random demand = new Random();
        private int angencyCreditCard;

        public TravelAgency(string name)
        {
            agencyName = name;
            //Applying for the new card as soon rooms are a good price.
            Int32 creditApplicationAmount = 200000;
            angencyCreditCard = BankService.centralBank.cardApplication(creditApplicationAmount);
        }

        /// <summary>Entry point for TravelAgency thread.</summary>
        /// <remarks>
        /// The thread will terminate after the Hotel thread has terminated.
        /// </remarks>
        public void getHotelRates()
        {
            for (Int32 i = 0; i < 10; i++)
            {
                Thread.Sleep(1000);
                string confirmation = MultiCellBuffer.hotel2agency.getCell(Name);
                if(confirmation != MultiCellBuffer.COME_BACK_LATER)
                {
                    Console.WriteLine("[Agency {0}] {1}", Name, confirmation);
                }
            } // TODO: exit when Hotel threads are terminated
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
        /// <param name="hotelName">Name of the hotel that has the discount</param>
        public void discountRooms(string hotelName, Int32 previousPrice, Int32 p)
        {
            Int32 demand = howManyRoomsToOrder(p, previousPrice);
            string encodedString = "";

            Console.WriteLine("[Agency {0}] Notified that hotel {1} has rooms at ${2}", Name, hotelName, p);
            OrderObject purchaseOrder = new OrderObject();
            purchaseOrder.amount = numberOfRooms.Next(0, 500);
            purchaseOrder.unitPrice = p;
            //Sender is this travel agency
            purchaseOrder.senderID = Name;
            //Send the order to the hotel that has the discount
            purchaseOrder.receiverID = hotelName;

            //Applying for the new card as soon rooms are a good price.
            purchaseOrder.cardNo = angencyCreditCard;

            DateTime now = DateTime.Now;
            purchaseOrder.timestamp = now.ToShortDateString() + " " + now.ToShortTimeString();

            //Sends this orderObject to be encoded
            encodedString = Coder.Encode(purchaseOrder);

            Console.WriteLine("[Agency {0}] Places an order with hotel {1} for {2} rooms",
                                Name, hotelName, purchaseOrder.amount);

            MultiCellBuffer.agency2hotel.setCell(hotelName, encodedString);
        }

       /// <summary>
       /// Calculate the number of rooms to order based on price differences and room demand.
       /// </summary>
       /// <param name="newPrice"></param>
       /// <param name="oldPrice"></param>
       /// <returns>The number of rooms that will be ordered.</returns>
        private int howManyRoomsToOrder(Int32 newPrice, Int32 oldPrice)
        {
            Int32 numberOfRooms = 0;
            Int32 priceDif = oldPrice - newPrice;
            Int32 currentDemand = demand.Next(0, 10);
            if (priceDif > 400)
            {
                numberOfRooms = 4;
            }
            else if (priceDif > 200 && priceDif < 400)
            {
                numberOfRooms = 3;
            }
            else if (priceDif > 100 && priceDif < 200)
            {
                numberOfRooms = 2;
            }
            else
            {
                numberOfRooms = 1;
            }
            numberOfRooms = numberOfRooms * currentDemand;
            return numberOfRooms;
        }

    }

    public class MyApplication
    {
        static void Main(string[] args)
        {
            // -- Initialize all the objects --
            // Initialize first to make sure we don't end up in weird states
            // when starting threads before everything is setup

            Hotel[] hotels = new Hotel[3];
            hotels[0] = new Hotel("Hyatt");
            hotels[1] = new Hotel("Hilton");
            hotels[2] = new Hotel("Ramada");

            TravelAgency[] travelAgencies = new TravelAgency[5];
            travelAgencies[0] = new TravelAgency("Kruise");
            travelAgencies[1] = new TravelAgency("Sunrunner");
            travelAgencies[2] = new TravelAgency("Winds");
            travelAgencies[3] = new TravelAgency("Colonial");
            travelAgencies[4] = new TravelAgency("Chanteclair");

            // connect the events
            for (int i = 0; i < travelAgencies.Length; i++)
            {
                for (int j = 0; j < hotels.Length; j++)
                {
                    hotels[j].promotionalEvent += new priceCutEvent(travelAgencies[i].discountRooms);
                }
            }

            // -- Now start all the threads --
            // one thread for each instance

            // start travel agency first to they'll be ready for any price cuts or confirmations
            for (int i = 0; i < 5; i++)
            {
                Thread travelAgencyThread = new Thread(new ThreadStart(travelAgencies[i].getHotelRates));
                travelAgencyThread.Name = String.Format("TravelAgency{0} {1}", i + 1, travelAgencies[i].Name);
                travelAgencyThread.Start();
            }

            // finally hotels
            for (int i = 0; i < 3; i++)
            {
                Thread hotelThread = new Thread(new ThreadStart(hotels[i].HotelAdvertiseFunc));
                hotelThread.Name = String.Format("Hotel{0} {1}", i + 1, hotels[i].Name);
                hotelThread.Start();
            }

        }
    }


    // Multi String Buffer
    //      Fix it if you know it!

    // Need semaphore/write/read for travel agent access
    // can't just read for hotel, but if hotel finds one, it can erase
    public class MultiCellBuffer
    {
        public static MultiCellBuffer agency2hotel = new MultiCellBuffer(3);
        public static MultiCellBuffer hotel2agency = new MultiCellBuffer(5);

        public const string COME_BACK_LATER = "cbl";

        private BufferCell[] cell;
        private Int32 cellsInUse = 0;
        private Semaphore _pool;
        private ReaderWriterLock rwLock;
        private int cellCount;

        private MultiCellBuffer(int cellCount)
        {
            this.cellCount = cellCount;
            cell = new BufferCell[cellCount];
            _pool = new Semaphore(cellCount, cellCount);
            rwLock = new ReaderWriterLock();
        }

        public bool checkFull() { return (cellsInUse >= cellCount); } // true if there's no free cell
        public bool checkEmpty() { return (cellsInUse == 0); } // true if there's nothin there

        // Hotel checking the orders
        public string getCell(string receiver)
        {
            string ret = COME_BACK_LATER;  // come back later
            rwLock.AcquireReaderLock(100);
            try
            {
                // Check each cell for price
                if (checkEmpty()) { } // if it's empty, try again in a bit
                else
                {
                    for (int i = 0; i < cellCount; ++i)
                    {
                        if (cell[i] != null)
                        {
                            string toChk = cell[i].receiver;
                            if (String.Compare(receiver, toChk) == 0)
                            {
                                string tmp = cell[i].value;
                                cell[i] = null;
                                --cellsInUse;
                                ret = tmp;
                            }
                        }
                    }
                }
            }
            finally
            {
                if (cellsInUse > cellCount)
                    cellsInUse = cellCount;
                else if (cellsInUse < 0)
                    cellsInUse = 0;

                rwLock.ReleaseReaderLock();
            }

            return ret;
        }

        // Travel agent posting orders
        public int setCell(string receiver, string order)
        {
            _pool.WaitOne();
            // wait forever until other threads are done with their work 
            // (should never deadlock unless another thread never leaves)
            rwLock.AcquireWriterLock(Timeout.Infinite);
            try
            {
                // Check each cell for price
                if (checkFull())
                    return 0;  // if it's full

                ++cellsInUse;
                cell[cellsInUse - 1] = new BufferCell(receiver, order);
            }
            finally
            {
                if (cellsInUse > cellCount)
                    cellsInUse = cellCount;
                else if (cellsInUse < 0)
                    cellsInUse = 0;

                rwLock.ReleaseWriterLock();
                _pool.Release();
            }
            return 1;
        }
    }

    // Bank service; holds the amounts in the bank account and decrypts credit card no's
    public class BankService
    {
        public static BankService centralBank = new BankService();

        private const int MAX_CARD_ACCOUNTS = 5;

        private double[] accountAmount;
        private int[] cardNumber;
        private object[] lck;

        public BankService()
        {
            accountAmount = new double[MAX_CARD_ACCOUNTS] {0,0,0,0,0};
            cardNumber = new int[MAX_CARD_ACCOUNTS] {-1,-1,-1,-1,-1};
            lck = new object[MAX_CARD_ACCOUNTS];
            for (int i = 0; i < MAX_CARD_ACCOUNTS; i++)
            {
                lck[i] = new object();
            }
        }

        /// <summary>Charge the given credit card the amount given.</summary>
        /// <remarks>
        /// The bank charges the account and returns the message "valid" if the account exists and
        /// the funds are sufficient for the purchase, otherwise, it returns "not valid"
        /// </remarks>
        /// <returns>"valid" or "not valid"</returns>
        public string chargeAccount(string cardNo, double amount) // card is encrypted
        {
            Project2.EncryptSvc.ServiceClient client = new Project2.EncryptSvc.ServiceClient();
            int cnum = Convert.ToInt32(client.Decrypt(cardNo));

            int j = 0;
            while (j < MAX_CARD_ACCOUNTS && (cnum != cardNumber[j]))
                ++j;

            string result;

            if (j >= MAX_CARD_ACCOUNTS) 
            {
                // account not found
                result = "not valid";
            }
            else
            {
                lock (lck[j])
                {
                    if (amount <= accountAmount[j])
                    {
                        accountAmount[j] -= amount;
                        result = "valid";
                    }
                    else
                    {
                        // insufficient funds
                        result = "not valid";
                    }
                }
            }

            return result;
        }

        public int cardApplication(int amount) // outs the card num
        {
            //Console.WriteLine("Bank application");
            int i = 0;
            while (i < MAX_CARD_ACCOUNTS && cardNumber[i] != -1)
                ++i;
            if (i >= MAX_CARD_ACCOUNTS)
            {
                // TODO: no free CC slots
                throw new NotImplementedException();
            }

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
    // entries separated by ','
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
            obj.timestamp = words[5];

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
        public string timestamp { get; set; }
    }
}
