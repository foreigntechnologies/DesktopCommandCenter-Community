using System;
using System.Reflection;
using Firebase.Auth;

class P { 
    static void Main() { 
        foreach(var m in typeof(FirebaseAuthClient).GetMethods()) { 
            Console.WriteLine(m.Name); 
        } 
    } 
}
