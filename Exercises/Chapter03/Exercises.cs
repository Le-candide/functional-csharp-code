using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using LaYumba.Functional;
using LaYumba.Functional.Option;
using NUnit.Framework;
using static LaYumba.Functional.F;

namespace Exercises.Chapter3
{
   public static class Enum
   {
      public static Option<T> Parse<T>(this string str) where T : struct
      {
         return System.Enum.TryParse(str, out T val) ? Some(val) : F.None;
      }
   }
   
   public static class Exercises
   {
      // 1 Write a generic function that takes a string and parses it as a value of an enum. It
      // should be usable as follows:

      // Enum.Parse<DayOfWeek>("Friday") // => Some(DayOfWeek.Friday)
      // Enum.Parse<DayOfWeek>("Freeday") // => None

      
      public static class Enum
      {
         public static Option<DayOfWeek> Parse(string str)
         {
            switch (str.ToLowerInvariant())
            {
               case "monday": return DayOfWeek.Monday;
               case "tuesday": return DayOfWeek.Tuesday;
               case "wednesday": return DayOfWeek.Wednesday;
               case "thursday": return DayOfWeek.Thursday;
               case "friday": return DayOfWeek.Friday;
               case "saturday": return DayOfWeek.Saturday;
               case "sunday": return DayOfWeek.Sunday;
               default: return F.None;
            }
         }
      }

      // 2 Write a Lookup function that will take an IEnumerable and a predicate, and
      // return the first element in the IEnumerable that matches the predicate, or None
      // if no matching element is found. Write its signature in arrow notation:

      // bool isOdd(int i) => i % 2 == 1;
      // new List<int>().Lookup(isOdd) // => None
      // new List<int> { 1 }.Lookup(isOdd) // => Some(1)

      public static Option<T> Lookup<T>(this IEnumerable<T> enumerable, Func<T, bool> pred)
      {
         foreach (var i in enumerable)
         {
            if (pred(i)) return Some(i);
         }

         return F.None;
      }


      // 3 Write a type Email that wraps an underlying string, enforcing that it’s in a valid
      // format. Ensure that you include the following:
      // - A smart constructor
      // - Implicit conversion to string, so that it can easily be used with the typical API
      // for sending emails

      public class Email
      {
         private static Regex Check = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
         private string _address;

         private Email(string str)
         {
            _address = str;
         }
         
         public static Option<Email> FromString(string str) => Check.IsMatch(str) ? Some(new Email(str)) : F.None;

         public static implicit operator string(Email email) => email._address;
      }

      // 4 Take a look at the extension methods defined on IEnumerable inSystem.LINQ.Enumerable.
      // Which ones could potentially return nothing, or throw some
      // kind of not-found exception, and would therefore be good candidates for
      // returning an Option<T> instead?
   }

   // 5.  Write implementations for the methods in the `AppConfig` class
   // below. (For both methods, a reasonable one-line method body is possible.
   // Assume settings are of type string, numeric or date.) Can this
   // implementation help you to test code that relies on settings in a
   // `.config` file?
   public class AppConfig
   {
      NameValueCollection source;

      //public AppConfig() : this(ConfigurationManager.AppSettings) { }

      public AppConfig(NameValueCollection source)
      {
         this.source = source;
      }

      public Option<string> Get(string name)
      {
         return source.Get(name) == null ? F.None : Some(source.Get(name));
      }

      public string Get(string name, string defaultValue)
      {
         return source.Get(name) == null ? defaultValue : source.Get(name);
      }
   }
   
   public static class Tests
   {
      [TestCase("murashov.ilya@gmail.com", ExpectedResult = true)]
      [TestCase("murashov.ilya@gmail..com", ExpectedResult = false)]
      public static bool Test(string address)
      {
         return Exercises.Email.FromString(address).Match(() => false, (_) => true);
      }
   }
}
