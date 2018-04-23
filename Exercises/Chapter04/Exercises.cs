using System;
using System.Collections.Generic;
using System.Linq;
using LaYumba.Functional;
using LaYumba.Functional.Option;

namespace Exercises.Chapter4
{
   static class Exercises
   {
      // 1 Implement Map for ISet<T> and IDictionary<K, T>. (Tip: start by writing down
      // the signature in arrow notation.)

      public static ISet<R> Map<T, R>(this ISet<T> set, Func<T, R> func)
      {
         return set.Select(func).ToHashSet();
      }

      public static IDictionary<Tkey, Rval> Map<Tkey, Tval, Rval>(this IDictionary<Tkey, Tval> dictionary, Func<Tval, Rval> func)
      {
         return dictionary.Select((kvp) => (kvp.Key, func(kvp.Value))).ToDictionary(tup => tup.Item1, tup => tup.Item2);
      }

      // 2 Implement Map for Option and IEnumerable in terms of Bind and Return.

      public static Option<R> Map2<T, R>(this Option<T> opt, Func<T, R> func)
      {
         return opt.Bind(t => F.Some(func(t)));
      }

      public static IEnumerable<R> Map2<T, R>(this IEnumerable<T> ts, Func<T, R> func)
      {
         return ts.Bind(t => F.List(func(t)));
      }

      // 3 Use Bind and an Option-returning Lookup function (such as the one we defined
      // in chapter 3) to implement GetWorkPermit, shown below. 

      // Then enrich the implementation so that `GetWorkPermit`
      // returns `None` if the work permit has expired.

      static Option<WorkPermit> GetWorkPermit(Dictionary<string, Employee> people, string employeeId)
      {
         var employee = people.Lookup(employeeId);
         return employee.Bind(e => e.WorkPermit);
      }

      // 4 Use Bind to implement AverageYearsWorkedAtTheCompany, shown below (only
      // employees who have left should be included).

      static double AverageYearsWorkedAtTheCompany(List<Employee> employees)
      {
         // your implementation here...
         return employees.Bind(e => e.LeftOn.Select(left => YearsBetween(e.JoinedOn, left)))
            .Average();
      }

      private static double YearsBetween(DateTime start, DateTime end) => (start - end).TotalDays;
   }

   public struct WorkPermit
   {
      public string Number { get; set; }
      public DateTime Expiry { get; set; }
   }

   public class Employee
   {
      public string Id { get; set; }
      public Option<WorkPermit> WorkPermit { get; set; }

      public DateTime JoinedOn { get; }
      public Option<DateTime> LeftOn { get; }
   }
}