// See https://aka.ms/new-console-template for more information
string[] myStringArray = new string[1];

myStringArray[0] = "Guac";
Console.WriteLine(myStringArray[0]);
Console.WriteLine(myStringArray[1]);

Dictionary<string, string[]> myGroceryDictionary = new Dictionary<string, string[]>{
    {"Dairy" , new string[]{"Cheese", "Milk"}}
};