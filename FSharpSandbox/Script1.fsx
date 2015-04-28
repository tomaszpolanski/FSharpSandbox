#r "../packages/FSharp.Data.2.1.0/lib/net40/FSharp.Data.dll"
#load @"../packages/FSharp.Charting.0.90.7/FSharp.Charting.fsx"
#load "../Portable/Extensions.fs"

open FSharp.Data
open FSharp.Charting

///////

let free =  FreebaseData.GetDataContext();

let elements = free.``Science and Technology``.Chemistry.``Chemical Elements``

let all = elements |> Seq.toList
printfn "Elements found: %d" (Seq.length all)

let hydrogen = elements.Individuals.Hydrogen 
printfn "Atominc number: %A" hydrogen.``Atomic number``

///////


let allCountries = seq { for country in free.Commons.Location.Countries do
                            yield country } 

let currency  = allCountries |> Seq.take 10
                             |> Seq.map (fun country -> country.``Currency Used`` |> Seq.tryHead )
                             |> Seq.filter (fun someCurrency -> someCurrency.IsSome)
                             |> Seq.map (fun someCurrency -> someCurrency.Value)
                             |> Seq.distinct
                             |> Seq.toList

let minimumWage  = allCountries |> Seq.take 100
                                |> Seq.map (fun country -> country.Name, country.``Minimum wage`` |> Seq.tryHead )
                                |> Seq.filter (fun (_, someWage) -> someWage.IsSome)
                                |> Seq.filter (fun (_, someWage) -> someWage.Value.Currency.Name = "Euro" )
                                |> Seq.map (fun (country, someWage) -> country, someWage.Value.Amount.Value)
                                |> Seq.sortBy (fun (_, amount) -> amount)
                                |> Seq.toList

Chart.Column(minimumWage).With3D()

////////

let data = WorldBankData.GetDataContext()


let dataChart = seq { for country in data.Countries do
                        let data = country.Indicators.``Condom use, population ages 15-24, male (% of males ages 15-24)``
                        let anyData = data.Values :> seq<float> |> Seq.filter (fun x -> x > 0.) 
                                                                |> Seq.exists
                        if anyData then yield Chart.Line(data, country.Name)  }

Chart.Combine(dataChart).WithLegend()

///////


let condom = data.Countries |> Seq.toList
                            |> List.filter (fun country -> country.Indicators.``Condom use, population ages 15-24, male (% of males ages 15-24)``.[2010] > 0.)
                            |> List.map (fun country -> (country.Name, country.Indicators.``Condom use, population ages 15-24, male (% of males ages 15-24)``.[2010]))



Chart.Pie(condom).With3D()

/////

let testData = data.Countries.Poland.Indicators.``GDP growth (annual %)`` 
              |> Seq.skipWhile (fun (a, b) ->  b < 0.)

Chart.Line(  testData)

////////

let countries = 
  [ data.Countries.``United States``; 
    data.Countries.Poland;
    data.Countries.``Burkina Faso``; 
    data.Countries.Niger; ]

let test = [ for c in countries -> c.Name, c.Indicators.``Armed forces personnel, total``.[2011] / 1000. ]

Chart.Pie test

////////

Chart.Combine([ for country in countries ->
                    let data = Seq.skip 20 country.Indicators.``Armed forces personnel, total`` 
                    Chart.Line(data, Name=country.Name) ]).WithLegend()
let tup = ( "adsf" , 1)

let _, _ = tup;;

///////

Chart.Combine([ for country in countries ->
                    let data = country.Indicators.``Birth rate, crude (per 1,000 people)``
                    Chart.Line(data, Name=country.Name) ])
     .WithTitle("Birth rate, crude (per 1,000 people)") 
     .WithLegend()

//Plot maternal mortality rates of selected countries
//Note: run the two plots separately to see their results
Chart.Combine([ for country in countries ->
                    let data = country.Indicators.``Maternal mortality ratio (national estimate, per 100,000 live births)``
                    Chart.Line(data, Name=country.Name) ])
     .WithTitle("Maternal Mortality Ratio")
     .WithYAxis(Min = 0., Max = 800.)
     .WithLegend()

//////

let isprime n =
    let rec check i =
        i > n/2 || (n % i <> 0 && check (i + 1))
    check 2

let t = isprime 84

//////

type Tree<'a> =
   | Tree of 'a * Tree<'a> * Tree<'a>
   | Leaf of 'a

// inorder : Tree<'a> -> seq<'a>    
let rec inorder tree =
    seq {
      match tree with
          | Tree(x, left, right) ->
               yield! inorder left
               yield x
               yield! inorder right
          | Leaf x -> yield x
    }   

let mytree = Tree(6, Tree(2, Leaf(1), Leaf(3)), Leaf(9)) 
let seq1 = inorder mytree
printfn "%A" seq1


//////
open Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols

let oneSecond = 1.0<s>
let fiveMeters = 5.0<m>

let something = oneSecond * fiveMeters;
/////

let g = 9.8<m/s^2>
let mass = 100.0<kg>
let force : float<N> = mass * g


/////

[<Measure>]
type C

[<Measure>]
type F

let centToFahr t = 1.8<F/C> * t + 32.0<F>

centToFahr 20.<C>


////////

let rec factorial number = 
       if number = 0 then 1
                     else factorial (number - 1) * number

factorial 4

////

let rec pow number power = 
        if power = 0 then 1
                     else number * pow number (power - 1)

pow 2 8

/////

    // pattern matching for lists
let listMatcher aList = 
    match aList with
    | [] -> printfn "the list is empty" 
    | [first] -> printfn "the list has one element %A " first 
    | [_; second] -> printfn "list is  and %A"  second 
    | _ -> printfn "the list has more than two elements" 

///////

let som aList = 
        match aList with
        | [] -> []
        | x::xs ->  xs 

som [1..10] |> List.iter (printfn "value is %d. ")

///////

let rec sum aList = 
        match aList with
        | [] -> 0
        | x::xs -> x + sum xs

sum [1..100]

//////

    // sequences use curly braces
let seq2 = seq { yield "a"; yield "b" }

// sequences can use yield and 
// can contain subsequences
let strange = seq {
    // "yield! adds one element
    yield 1; yield 2;

    // "yield!" adds a whole subsequence
    yield! [5..10]  
    yield! seq {
        for i in 1..10 do 
          if i%2 = 0 then yield i }}
// test                
strange |> Seq.toList    

//////
type Person = {First:string; Last:string}

type Employee = 
  | Worker of Person
  | Manager of Employee list

let jdoe = {First="John";Last="Doe"}
let worker = Worker jdoe

//////


type Suit = Club | Diamond | Spade | Heart
type Rank = Two | Three | Four | Five | Six | Seven | Eight 
            | Nine | Ten | Jack | Queen | King | Ace    

let hand = [ Club,Ace; Heart,Three; Heart,Ace; 
             Spade,Jack; Diamond,Two; Diamond,Ace ]

// sorting
List.sort hand |> printfn "sorted hand is (low to high) %A"
List.max hand |> printfn "high card is %A"
List.min hand |> printfn "low card is %A"

/////////

let (|Digit|Letter|Whitespace|Other|) ch = 
       if System.Char.IsDigit(ch) then Digit
       else if System.Char.IsLetter(ch) then Letter
       else if System.Char.IsWhiteSpace(ch) then Whitespace
       else Other         

// ... and then use it to make parsing logic much clearer
let printChar ch = 
  match ch with
  | Digit -> printfn "%c is a Digit" ch
  | Letter -> printfn "%c is a Letter" ch
  | Whitespace -> printfn "%c is a Whitespace" ch
  | _ -> printfn "%c is something else" ch

// print a list
['a';'b';'1';' ';'-';'c'] |> List.iter printChar

//////

open System.Net
open System
open System.IO
open Microsoft.FSharp.Control.CommonExtensions   

// Fetch the contents of a URL asynchronously
let fetchUrlAsync url =        
    async {   // "async" keyword and curly braces 
              // creates an "async" object
        let req = WebRequest.Create(Uri(url)) 
        use! resp = req.AsyncGetResponse()    
            // use! is async assignment
        use stream = resp.GetResponseStream() 
            // "use" triggers automatic close()
            // on resource at end of scope
        use reader = new IO.StreamReader(stream) 
        let html = reader.ReadToEnd() 
        printfn "finished downloading %s" url 
        }

// a list of sites to fetch
let sites = ["http://www.bing.com";
             "http://www.google.com";
             "http://www.microsoft.com";
             "http://www.amazon.com";
             "http://www.yahoo.com"]

// do it
sites 
|> List.map fetchUrlAsync  // make a list of async tasks
|> Async.Parallel          // set up the tasks to run in parallel
|> Async.RunSynchronously  // start them off


/////////

type Color =
    | Red 
    | Green
    | Blue

let printColorName color =
    match color with
    | Color.Red -> printfn "Red"
    | Color.Green -> printfn "Green"
    | Color.Blue -> printfn "Blue"

printColorName Color.Red
printColorName Color.Green
printColorName Color.Blue


////////////////

#r "../packages/Rx-Interfaces.2.2.5/lib/Net45/System.Reactive.Interfaces.dll"
#r "../packages/Rx-Core.2.2.5/lib/Net45/System.Reactive.Core.dll"
#r "../packages/Rx-Linq.2.2.5/lib/Net45/System.Reactive.Linq.dll"

open System

open System.Linq
open System.Reactive.Linq

Observable.Interval(TimeSpan.FromSeconds 0.1).Subscribe (fun x -> printf "Something %d" x) |> ignore

//////

open System
open System.Threading

let myAsync =
        async {
            while true do
                do! Async.Sleep(1000)
                printf "%A" DateTime.Now
        }
let tokenSource1 = new System.Threading.CancellationTokenSource(TimeSpan.FromMilliseconds 10000.)
let val1 = Async.Start(myAsync, cancellationToken=tokenSource1.Token)  


////////
open System.Net
open Microsoft.FSharp.Control.WebExtensions

let urlList = [ "Microsoft.com", "http://www.microsoft.com/" 
                "MSDN", "http://msdn.microsoft.com/" 
                "Bing", "http://www.bing.com"
              ]

let fetchAsync(name, url:string) =
    async { 
        try 
            let uri = new System.Uri(url)
            let webClient = new WebClient()
            let! html = webClient.AsyncDownloadString(uri)
            printfn "Read %d characters for %s" html.Length name
        with
            | ex -> printfn "%s" (ex.Message);
    }

let runAll() =
    urlList
    |> Seq.map fetchAsync
    |> Async.Parallel 
    |> Async.RunSynchronously
    |> ignore

runAll()
/////////

#load "../Portable/Portable.fs"

Portable.Recursive.Factorials |> Seq.take 5 |> List.ofSeq |> Seq.iter ( fun factorial -> printf "%A" factorial )

Portable.Recursive.Fibonnaci |> Seq.take 10 |> List.ofSeq |> Seq.iter ( fun fibo -> printf "%A " fibo )

Portable.BigFibonnaci.fac |> Seq.take 6 |> Seq.iter (printfn "%A")

/////////

type Titanic = CsvProvider<"Titanic.csv">

let tytanicData = Titanic.Load "Titanic.csv"

tytanicData.Rows |> Seq.count (fun person -> person.Survived)

let peopleCount = tytanicData.Rows |> Seq.length;

let ageGroups = tytanicData.Rows |> Seq.countBy (fun person -> person.Age < 14.)
                                 |> Seq.map (fun ( isChild, count)  -> match isChild with 
                                                                           | true -> "Child", count
                                                                           | false -> "Adult", count )

Chart.Pie(ageGroups)

//let children, adults  = tytanicData.Rows |> Seq.toList |> List.partition (fun person -> person.Age < 14. )   



let survivedAgeGroups = tytanicData.Rows |> Seq.groupBy (fun person -> person.Survived)
                                         |> Seq.filter (fun (survived, _) -> survived)
                                         |> Seq.collect (fun ( _, survivors) -> survivors)
                                         |> Seq.countBy (fun person -> person.Age < 14.)
                                         |> Seq.map (fun ( isChild, count)  -> match isChild with 
                                                                                | true -> "Child", count
                                                                                | false -> "Adult", count )

Chart.Pie(survivedAgeGroups)

let allChildren = tytanicData.Rows |> Seq.count (fun person -> person.Age < 14.)


let adultsGroups = tytanicData.Rows |> Seq.filter (fun person -> person.Age > 14.)
                                      |> Seq.groupBy (fun person -> person.Survived)
                                      |> Seq.map (fun (groupName, list) -> match groupName with
                                                                                | true -> "Survived"
                                                                                | false -> "Not"
                                                                                , list |> Seq.length)

Chart.Doughnut(adultsGroups, Name="Adults")



////////
//http://ichart.finance.yahoo.com/table.csv?s=GOOG"
//http://ichart.finance.yahoo.com/table.csv?s=NOK"

type data = CsvProvider<"msft.csv">
let msft = data.Load("msft.csv")

msft.Rows |> Seq.map (fun day -> day.Date, day.Open)
          |> Chart.FastLine

open System

let lastDays = msft.Rows |> Seq.takeWhile (fun day -> day.Date > DateTime.Now.AddDays(-50.0)) 
                         |> Seq.map (fun day -> day.Date, day.High, day.Low, day.Open, day.Close)

let min = lastDays |> Seq.map (fun (_, _, low, _, _) -> low)
                     |> Seq.min
                     |> float


let max = lastDays |> Seq.map (fun (_, high, _, _, _) -> high )
                   |> Seq.max
                   |> float

Chart.Candlestick( lastDays).WithYAxis(Min = min, Max = max)

//////

let nokData = data.Load("http://ichart.finance.yahoo.com/table.csv?s=NOK")

let ms = msft.Rows |> Seq.map (fun day -> day.Date, day.Open)
                   |> Chart.FastLine

let nok = nokData.Rows |> Seq.map (fun day -> day.Date, day.Open)
                       |> Chart.FastLine

[ms; nok] |> Chart.Combine
          

//////////
type Countries = CsvProvider<"countrylist.csv">
let countryData = Countries.Load("countrylist.csv")

let currencies = countryData.Rows |> Seq.groupBy (fun country -> country.``ISO 4217 Currency Name``)
                                  |> Seq.map (fun (key, countries) -> (key, countries |> Seq.length))
                                  |> Seq.sortBy (fun (currency, count) -> -count)
                                  |> Seq.take 10
                                  |> Seq.filter (fun (currency, _) -> currency <> "")
                                  |> Seq.toList

Chart.Bar(currencies).With3D()

////////

type Kitten = { Name: string; Age: int }



let youngerThan age kitten = kitten.Age < age


let bigEvil = { Name = "Evil"; Age = 10}

let { Name = catsName } = bigEvil

youngerThan 85 bigEvil

let toLower (text : string) = text.ToLowerInvariant()

toLower bigEvil.Name

let littleEvil = { bigEvil with Name = toLower bigEvil.Name }

///////////

type CartItem = { ProductCode: string; Qty: int }
type Payment = Payment of float
type ActiveCartData = { UnpaidItems: CartItem list }
type PaidCartData = { PaidItems: CartItem list; Payment: Payment}

let paid = { PaidItems = [{ProductCode = "a"; Qty = 1}] ; Payment = Payment 4.}

paid |> string

type ShoppingCart = 
    | EmptyCart  // no data
    | ActiveCart of ActiveCartData
    | PaidCart of PaidCartData
    
let shopping = PaidCart  paid


//////////


let rec quickSort list =
    match list with
        | [] -> []
        | first::rest ->
            let smallerElements = 
                rest |> List.filter (fun item -> item < first)
                     |> quickSort
            let largerElements = 
                rest |> List.filter (fun item -> item >= first)
                     |> quickSort

            List.concat [smallerElements; [first]; largerElements]

printfn "%A" (quickSort [1;5;23;18;9;1;3])

let rec quicksort2 = function
   | [] -> []                         
   | first::rest -> 
        let smaller,larger = List.partition ((>=) first) rest 
        List.concat [quicksort2 smaller; [first]; quicksort2 larger]


//////

let listEven, listOdd = [ 1 .. 10 ] |> List.partition (fun elem -> elem % 2 = 0) 


//////


type Shape =        // define a "union" of alternative structures
| Circle of int 
| Rectangle of int * int
| Polygon of (int * int) list
| Point of (int * int) 

let draw shape =    // define a function "draw" with a shape param
  match shape with
  | Circle radius -> printfn "The circle has a radius of %d" radius
  | Rectangle (height,width) -> printfn "The rectangle is %d high by %d wide" height width
  | Polygon points -> printfn "The polygon is made of these points %A" points
  | _ -> printfn "I don't recognize this shape"

let circle = Circle(10)
let rect = Rectangle(4,5)
let polygon = Polygon( [(1,1); (2,2); (3,3)])
let point = Point(2,3)

[circle; rect; polygon; point] |> List.iter draw



///////

let aFun x = x + "a"

let bFun x = x + "b"

let abFun = aFun >> bFun

abFun "1"

"2" |> (aFun >> bFun)


let logMsg msg x = printf "%s%s" msg x; x     //without linefeed 
let logMsgN msg x = printfn "%s%s" msg x; x   //with linefeed

// new composed function with new improved logging!
let funLog = 
   logMsg "before=" 
   >> aFun 
   >> logMsg " after a=" 
   >> bFun
   >> logMsgN " result=" 

[1..10] |> List.map string |> List.map funLog //apply to a whole list

let listOfFunctions = [
   aFun; 
   bFun;
   logMsgN "result=";
   ]

// compose all functions in the list into a single one
let allFunctions = listOfFunctions |> List.reduce (>>)  

//test
allFunctions "3"

////////////

let add x y = x + y

let add42 = add 42

add42 5

//////////////

// define a "union" of two different alternatives
type Result<'a, 'b> = 
    | Success of 'a  // 'a means generic type. The actual type
                     // will be determined when it is used.
    | Failure of 'b  // generic failure type as well

// define all possible errors
type FileErrorReason = 
    | FileNotFound of string
    | UnauthorizedAccess of string * System.Exception

// define a low level function in the bottom layer
let performActionOnFile action filePath =
   try
      //open file, do the action and return the result
      use sr = new System.IO.StreamReader(filePath:string)
      let result = action sr  //do the action to the reader
      sr.Close()
      Success (result)        // return a Success
   with      // catch some exceptions and convert them to errors
      | :? System.IO.FileNotFoundException as ex 
          -> Failure (FileNotFound filePath)      
      | :? System.Security.SecurityException as ex 
          -> Failure (UnauthorizedAccess (filePath,ex))  
      // other exceptions are unhandled


let middleLayerDo action filePath = 
    let fileResult = performActionOnFile action filePath
    // do some stuff
    fileResult //return

// a function in the top layer
let topLayerDo action filePath = 
    let fileResult = middleLayerDo action filePath
    // do some stuff
    fileResult //return

let printFirstLineOfFile filePath = 
    let fileResult = topLayerDo (fun fs->fs.ReadLine()) filePath

    match fileResult with
    | Success result -> 
        // note type-safe string printing with %s
        printfn "first line is: '%s'" result   
    | Failure reason -> 
       match reason with  // must match EVERY reason
       | FileNotFound file -> 
           printfn "File not found: %s" file
       | UnauthorizedAccess (file,_) -> 
           printfn "You do not have access to the file: %s" file

let writeSomeText filePath someText = 
    use writer = new System.IO.StreamWriter(filePath:string)
    writer.WriteLine(someText:string)
    writer.Close()

let goodFileName = "good.txt"
let badFileName = "bad.txt"

writeSomeText goodFileName "hello"


#time
printFirstLineOfFile goodFileName 
#time

printFirstLineOfFile badFileName 

////////////////
open System
//open Microsoft.FSharp.Control  // Async.* is in this module.

let userTimerWithAsync = 

    // create a timer and associated async event
    let timer = new System.Timers.Timer(4000.0)
    let timerEvent = Async.AwaitEvent (timer.Elapsed) |> Async.Ignore

    // start
    printfn "Waiting for timer at %O" DateTime.Now.TimeOfDay
    timer.Start()

    // keep working
    printfn "Doing something useful while waiting for event"

    // block on the timer event now by waiting for the async to complete
    Async.RunSynchronously timerEvent

    // done
    printfn "Timer ticked at %O" DateTime.Now.TimeOfDay


//////

let (i1success,i1) = System.Int32.TryParse("123");
if i1success then printfn "parsed as %i" i1 else printfn "parse failed"

let (d1success,d1) = System.DateTime.TryParse("1/1/1980");
let (d2success,d2) = System.DateTime.TryParse("hello");

///////

let makeResource name = 
   { new System.IDisposable 
     with member this.Dispose() = printfn "%s disposed" name }

(makeResource "test" ).Dispose()

///////


type IAnimal = 
   abstract member MakeNoise : unit -> string

let showTheNoiseAnAnimalMakes (animal:IAnimal) = 
   animal.MakeNoise() |> printfn "Making noise %s" 

type Cat = Felix | Socks
type Dog = Butch | Lassie 

type Cat with
   member this.AsAnimal = 
        { new IAnimal 
          with member a.MakeNoise() = "Meow" }

type Dog with
   member this.AsAnimal = 
        { new IAnimal 
          with member a.MakeNoise() = "Woof" }

let dog = Lassie
showTheNoiseAnAnimalMakes (dog.AsAnimal)

let cat = Felix
showTheNoiseAnAnimalMakes (cat.AsAnimal)


//////
module Virtual =
    // interface
    type IEnumerator<'a> = 
        abstract member Current : 'a
        abstract MoveNext : unit -> bool 

    // abstract base class with virtual methods
    [<AbstractClass>]
    type Shape() = 
        //readonly properties
        abstract member Width : int with get
        abstract member Height : int with get
        //non-virtual method
        member this.BoundingArea = this.Height * this.Width
        //virtual method with base implementation
        abstract member Print : unit -> unit 
        default this.Print () = printfn "I'm a shape"

    // concrete class that inherits from base class and overrides 
    type Rectangle(x:int, y:int) = 
        inherit Shape()
        override this.Width = x
        override this.Height = y
        override this.Print ()  = printfn "I'm a Rectangle"

    //test
    let r = Rectangle(2,3)
    printfn "The width is %i" r.Width
    printfn "The area is %i" r.BoundingArea
    r.Print()

    ////////
module Generic = 
    // standard generics
    type KeyValuePair<'a,'b>(key:'a, value: 'b) = 
        member this.Key = key
        member this.Value = value
    
    // generics with constraints
    type Container<'a,'b 
        when 'a : equality 
        and 'b :> System.Collections.ICollection>
        (name:'a, values:'b) = 
        member this.Name = name
        member this.Values = values

///////
module Extensions = 

    type System.String with
        member this.StartsWithA = this.StartsWith "A"

//////////
module ``Parameter arrays`` = 

    open System
    type MyConsole() =
        member this.WriteLine([<ParamArray>] args: Object[]) =
            for arg in args do
                printfn "%A" arg    

    let cons = new MyConsole()
    cons.WriteLine("abc", 42, 3.14, true)

/////////

module ``Return value type`` =
    let stringLengthAsInt (x:string) :int = x.Length    

//////

module ``Functions as output`` =
    let adderGenerator numberToAdd = (+) numberToAdd

    let add1 = adderGenerator 1
    let add2 = adderGenerator 2

    add1 5    
    add2 5    

//////

module ``Using type annotations to constrain function types`` = 

    let evalWith5AsInt (fn:int->int) = fn 5
    let evalWith5AsFloat (fn:int->float) = fn 5

    let evalWith5AsString fn :string = fn 5

///////////

module Unit =
    let whatIsThis = ()


    let printHelloVal = printf "hello world"
    let printHelloFun() = printf "hello world"

////////////
module ``Curried functions`` =
    
    let add x = (+) x 
    let add2 x y = (+) x y

    add 1 2

module Method =

    let printHello() = printfn "hello"

module ``Extracting parameters`` =
    type Name = {first:string; last:string} // define a new type
    let bob = {first="bob"; last="smith"} // define a value 
 
    // single parameter style
    let f1 name = // pass in single parameter 
        let {first=f; last=l} = name // extract in body of function 
        printfn "first=%s; last=%s" f l
// match in the parameter itself
    let f2 {first=f; last=l} = // direct pattern matching 
        printfn "first=%s; last=%s" f l 

    f1 bob
    f2 bob

module ``Short functions`` =
    let add x y = x + y // explicit
    let add' x = (+) x // point free
 
    let add1Times2 x = (x + 1) * 2 // explicit
    let add1Times2' = (+) 1 >> (*) 2 // point free
 
    let sum list = List.reduce (fun sum e -> sum+e) list // explicit
    let sum' = List.reduce (+) // point free
/////////////////
 
type Adder = int -> int
type AdderGenerator = int -> Adder
//////////////////
 
module Test21 =

    // declare the type outside the module
    type PersonType = {FirstName:string; Last:string}
    // declare a module for functions that work on the type
    module Person = 
        // constructor
        let create first last = {FirstName=first; Last=last}
        // method that works on the type
        let fullName {FirstName=first; Last=last} = 
            first + " " + last
    // test
    let person = Person.create "john" "doe" 
    Person.fullName person |> printfn "Fullname=%s"

//////////////
module ``And Or matching`` =

    let y = 
        match (1,0) with 
        // OR -- same as multiple cases on one line
        | (2,x) | (3,x) | (4,x) -> printfn "x=%A" x 
        // AND -- must match both patterns at once
        // Note only a single "&" is used
        | (2,x) & (_,1) -> printfn "x=%A" x 
        | x -> printf "No match for %A" x

module ``function`` =
    let divideBy  top bottom  =
        if bottom = 0
        then None
        else Some(top/bottom)

    divideBy 6 2

    let devideBy' top = function
        | 0 -> None
        | x -> Some(top/x)

    devideBy' 8 2
 module ``id`` =
    let x = 1

    id x

module Infix =

    let (>>=) m f = 
        printfn "expression is %A" m
        f m

    let loggingWorkflow = 
        1 >>= (+) 2 >>= (*) 42 >>= id

    1 |> (+) 2 |> (*) 42 |> id

module Join =
    
    let join separator values = 
        let rec loop current = function
            | [] -> current
            | x::xs -> match current with
                        | "" -> loop (sprintf "%A" x) xs
                        | _  -> loop (sprintf "%s%s%A" current  separator x) xs
        loop "" values

    join ", " []

module ``Infinit loop`` = 
    let rec loop index : unit = 
        let max = 100000000
        match index with
        | x when x % (max / 10) = 0 -> 
            printfn "Number %d" x
            loop (x + 1)

        | x -> loop (x + 1)

    loop 1

module ``Property based testin`` =

    let rand = System.Random()
    let randInt() = rand.Next()

    type Expected = Expected of int
    type Acctual = Acctual of int
    type TestResult =
        | Success of string
        | Failure of string * Expected * Acctual
    
    let compare name (expected, acctual) = 
        match (expected, acctual) with
        | (Expected e, Acctual a) when e = a ->
            Success name
        | (Expected e, Acctual a) -> 
            Failure(name, expected, acctual)
    
    let takeLastWhile p (source: seq<_>) = 
        seq { use e = source.GetEnumerator() 
              let latest = ref Unchecked.defaultof<_>
              while e.MoveNext() && (latest := e.Current; p !latest) do 
                  yield !latest
              yield !latest }

    let repeatTest count test =
        seq { for _ in [0..count] do yield test() } 
        |> takeLastWhile (function Success _ -> true | Failure _ -> false)
        |> Seq.last

    let runTest = repeatTest 100

    let printTest test = 
        match runTest test with
            | Success name ->
                printfn "Pass: %A" name
            | Failure (name, expected, acctual) ->
                printfn "Failure: %A failed with %A, %A" name expected acctual

    let add x y = x + y
 
    let ``not order dependent test``() =
        let x, y = randInt(), randInt()

        let expected = Expected (add y x)
        let accutal = Acctual (add x y)
        compare "not order dependent test" (expected, accutal)

    let ``and twice one is the same as adding two``() =
        let x =  randInt()

        let expected = Expected (x |> add 2)
        let accutal = Acctual ( x |> add 1 |> add 1)
        compare "and twice one is the same as adding two" (expected, accutal)

    let ``zero is the same as doing nothing``() =
        let x = randInt()

        let expected = Expected (x)
        let accutal = Acctual (add x 0)
        compare "zero is the same as doing nothing" (expected, accutal)

    printTest ``not order dependent test``
    printTest ``zero is the same as doing nothing``
    printTest ``and twice one is the same as adding two``

    