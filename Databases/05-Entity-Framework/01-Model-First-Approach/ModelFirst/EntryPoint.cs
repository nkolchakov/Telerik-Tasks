using System;
using System.Linq;

namespace ModelFirst
{
    class EntryPoint
    {
        public static void InsertCustomer(Customer ct)
        {
            using (var db = new NorthwindEntities())
            {
                if (db.Customers.Any(c => c.CustomerID == ct.CustomerID))
                {
                    Console.WriteLine("Provided Customer already exists");
                    return;
                }
                db.Customers.Add(ct);
                db.SaveChanges();

                Console.WriteLine($"Customer with ID:{ct.CustomerID} added");
            }
        }

        public static void ModifyCustomer(Customer ct)
        {
            using (var db = new NorthwindEntities())
            {

                var result = db.Customers
                    .Where(c => c.CustomerID == ct.CustomerID)
                    .FirstOrDefault();

                if (result == null)
                {
                    Console.WriteLine($"No such customer with ID: {ct.CustomerID}!");
                    return;
                }

                // copy modified customer properties values to database's customer so it can update itself
                foreach (var property in typeof(Customer).GetProperties())
                {
                    property.SetValue(result, property.GetValue(ct, null), null);
                }

                db.SaveChanges();
                Console.WriteLine($"{ct.CustomerID} modified");
            }
        }

        public static void RemoveCustomer(Customer ct)
        {
            using (var db = new NorthwindEntities())
            {
                Customer elToRemove;

                elToRemove = db.Customers
                                .FirstOrDefault(c => c.CustomerID == ct.CustomerID);

                if (elToRemove == null)
                {
                    Console.WriteLine($"Customer with ID: {ct.CustomerID} doesn't exist");
                }
                db.Customers.Remove(elToRemove);
                db.SaveChanges();
                Console.WriteLine("Deleted");
            }
        }

        public static void AllCustomers(int year, string toCountry)
        {
            using (var db = new NorthwindEntities())
            {
                var result = db.Orders
                               .Where(o => o.RequiredDate.Value.Year == year &&
                                      o.ShipCountry == toCountry)
                                .Select(
                                    r => new
                                    {
                                        CustomerName = r.Customer.CompanyName
                                    })
                                .ToList();

                foreach (var item in result)
                {
                    Console.WriteLine(item.CustomerName);
                }
            }
        }

        public static void AllCustomersNativeSql()
        {
            using (var db = new NorthwindEntities())
            {
                db.Database.SqlQuery<CustomerModel>("SELECT o.CustomerID, c.CompanyName " +
                                                    "FROM Customers c " +
                                                    "JOIN Orders o " +
                                                    "ON c.CustomerID = o.CustomerID " +
                                                    "WHERE o.ShipCountry = 'Canada' AND YEAR(o.OrderDate) = 1997 ")
                                         .ToList()
                                         .ForEach(x => Console.WriteLine($"{x.CustomerID} - {x.CompanyName}"));
            }
        }

        public static void AllSalesByRegionAndPeriod(string region, DateTime start, DateTime end)
        {
            using (var db = new NorthwindEntities())
            {
                var result = db.Orders
                               .Where(o => o.ShipRegion == region && 
                                     (o.OrderDate >= start && o.ShippedDate <= end)
                                )
                               .Select(x => new
                               {
                                   x.OrderID,
                                   x.Customer.CompanyName
                               })
                               .ToList();

                foreach (var item in result)
                {
                    Console.WriteLine($"ID: {item.OrderID} CompanyName: {item.CompanyName}");
                }
            }
        }

        static void Main()
        {
            using (var db = new NorthwindEntities())
            {
                var newCustomer = new Customer()
                {
                    CustomerID = "Rndom",
                    CompanyName = "HybridComp"
                };

                // InsertCustomer(newCustomer);

                var modifiedCustomer = new Customer()
                {
                    CustomerID = "Rndom",
                    CompanyName = "Modified Hybrid",
                    City = "Sofia"
                };

                // ModifyCustomer(modifiedCustomer);

                // RemoveCustomer(newCustomer); // or pass modifiedCustomer

                // AllCustomers(1997,"Canada");

                // AllCustomersNativeSql();

                // AllSalesByRegionAndPeriod("RJ", new DateTime(1996, 07, 08), new DateTime(1996, 08, 01));
                
                
            }
        }
    }
}
