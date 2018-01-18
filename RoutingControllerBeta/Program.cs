﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RoutingControllerBeta
{
    class Program
    {
        static void Main(string[] args)
        {


            string subNetworkCallSign = "SN.0";
            string autonomicNetworkCallSign = "AS.0";
            int RCReqPort = 0;
            int RCResPort = 0;
            int mainPort = 0;
            string addressesFileName = "addresses.txt";
            string name = "RC.0";

            int whichTable =0;

            foreach (string arg in args)
            {
                string[] param = arg.Split('=');

                if (param[0].Equals("SBN"))
                    subNetworkCallSign = param[1];

                if (param[0].Equals("AS"))
                    autonomicNetworkCallSign = param[1];

                if (param[0].Equals("addressesFileName"))
                    addressesFileName = param[1];

                if (param[0].Equals("name"))
                    name = param[1];

                if (param[0].Equals("RCReqPort"))
                    RCReqPort = Int32.Parse(param[1]);

                if (param[0].Equals("RCResPort"))
                    RCResPort = Int32.Parse(param[1]);

                if (param[0].Equals("mainPort"))
                    mainPort = Int32.Parse(param[1]);

                if (param[0].Equals("TEST"))
                    whichTable = Int32.Parse(param[1]);
            }

            Console.WriteLine(subNetworkCallSign + " || " + autonomicNetworkCallSign + " || " + mainPort);


            string addressesPath = Environment.CurrentDirectory + "\\" + addressesFileName;
            AddressBook addressBook = new AddressBook(addressesPath);

            Console.WriteLine("After loading addresses");

            LinkTable linkTable = new LinkTable();

            if (whichTable == 1)
            {
                linkTable = new LinkTable();
                linkTable.add(new Link("R.1", "SN.1", "AS.1", 1, "R.3", "SN.1", "AS.1", 1, 100));
                linkTable.add(new Link("R.1", "SN.1", "AS.1", 1, "R.2", "SN.1", "AS.1", 1, 25));

                linkTable.add(new Link("R.2", "SN.1", "AS.1", 1, "R.9", "SN.3", "AS.3", 1, 50));
                linkTable.add(new Link("R.2", "SN.1", "AS.1", 1, "R.11", "SN.1", "AS.1", 1, 10));

                linkTable.add(new Link("R.3", "SN.1", "AS.1", 1, "R.4", "SN.2", "AS.1", 1, 100));
                linkTable.add(new Link("R.3", "SN.1", "AS.1", 1, "R.7", "SN.1", "AS.1", 1, 12));

                linkTable.add(new Link("R.7", "SN.1", "AS.1", 1, "R.10", "SN.3", "AS.3", 1, 50));
                linkTable.add(new Link("R.7", "SN.1", "AS.1", 1, "R.8", "SN.1", "AS.1", 1, 25));

                linkTable.add(new Link("R.8", "SN.1", "AS.1", 1, "R.10", "SN.3", "AS.3", 1, 100));
                linkTable.add(new Link("R.8", "SN.1", "AS.1", 1, "R.5", "SN.2", "AS.1", 1, 50));

                linkTable.add(new Link("R.11", "SN.1", "AS.1", 1, "R.8", "SN.1", "AS.1", 1, 15));

                Console.WriteLine("Loaded table 1");

            }
            else if (whichTable == 2)
            {
                linkTable = new LinkTable();
                linkTable.add(new Link("R.4", "SN.2", "AS.1", 1, "R.5", "SN.2", "AS.1", 1, 40));
                linkTable.add(new Link("R.4", "SN.2", "AS.1", 1, "R.6", "SN.2", "AS.1", 1, 50));
                linkTable.add(new Link("R.4", "SN.2", "AS.1", 1, "R.2", "SN.1", "AS.1", 1, 100));

                linkTable.add(new Link("R.5", "SN.2", "AS.1", 1, "R.7", "SN.1", "AS.1", 1, 100));
                linkTable.add(new Link("R.5", "SN.2", "AS.1", 1, "R.6", "SN.2", "AS.1", 1, 100));

                linkTable.add(new Link("R.6", "SN.2", "AS.1", 1, "R.11", "SN.1", "AS.1", 1, 20));
                linkTable.add(new Link("R.6", "SN.2", "AS.1", 1, "R.8", "SN.1", "AS.1", 1, 15));

                Console.WriteLine("Loaded table 2");
            }

            if (RCReqPort != 0)
            {
                RCRequestHandler handler = new RCRequestHandler(RCReqPort, linkTable, subNetworkCallSign, autonomicNetworkCallSign);
                Thread RCRequestHandlerThread = new Thread(() => handler.run());
                RCRequestHandlerThread.Start();
                Console.WriteLine("RC listening preparation finished");
            }

            RouteRequestResolver routeRequestResolver = new RouteRequestResolver(addressBook, RCResPort, linkTable, subNetworkCallSign, autonomicNetworkCallSign, name);
            LinkTableUpdater linkTableUpdater = new LinkTableUpdater();

            MessageListener messageListener = new MessageListener(linkTableUpdater, routeRequestResolver, mainPort);
            messageListener.run();




            




            
            

        }
    }
}