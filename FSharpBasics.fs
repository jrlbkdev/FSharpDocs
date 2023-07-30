module FSharpIntro

/////////////////////////////////////
// Basic concepts and language philosophy
/////////////////////////////////////

// F# code is written top-down. 
// In other words, code only has access to code that has been defined before

// F# code is also white space sensitive
// To create a scope, simply use indentation. This is useful to create intermediate values
// and helper functions

// F# is statically typed. At compile-time, the compiler always knows the type of any given value
// However, F# has a powerful type inference engine so it's able to "know" the type of values
// automatically in many cases

// Everything is an expression in F#, three are no statements
// Even things that look like statements in reality return an "empty" value, named unit
printfn "Hi from F#" // returns unit

// Any of this code can be run within an interactive environment
// that can be open by running dotnet fsi
// Alternatively, the file can be loaded in visual studio and
// then code can be sent to F# interactive by selecting a block of code
// and pressing alt-Enter

////////////////////////////////////////////
// Simple values and data structures
////////////////////////////////////////////

// defining simple values: immutable by default

let i = 3

let f = 1.5
let g = 2.

let s = "a character string"

let n: int = 53 // explicit typing
let (n2: int) = 30 // sometimes parenthesis will be required for type annotations


// tuples: groups of values separated by commas. parentheses are usually optional

let pair = (6, 22)
let tupleOf3 = (1, 2, 3)
let tupleOfStrings = "first", "second", "third" 

// decomposing tuples
let (first, second) = pair // first: 6. second: 22


/////////////////////////////////////
// Functions
/////////////////////////////////////
// F# functions can be defined in a variety of different ways 

// 1. Arguments are separated by white space
let sum1 x y = x + y
    
// 2: Arguments are separated by commas (forming a single tuple)
let sum2 (x, y) = x + y

// 3. They can also be defined "inline" by using the fun keyword
// this example is identical to the first way
// This way is equivalent to C#'s lambda functions
let sum1Inline = fun x y -> x + y // equivalent to sum1
let sum2Inline = fun (x, y) -> x + y // equivalent to sum2

// Both are used in practice, and they're not equivalent though
// explaining why requires a bit of knowledge about functional programming (currying and partial application)
// moving on from that...

// invoking functions is straightforwards
let sumResult1 = sum1 5 10 // 15
let sumResult2 = sum2 (5, 10) // 15

// sometimes type annotations are needed
let printStr (str: string) = 
    // without annotation, the compiler can't know that str is a string
    // and there's a property called Length defined for strings
    printfn $"Length: {str.Length}"

// if you're not familiar with functional programming, this will melt your brain a bit
// functions are values just like any other
// they can be passed to functions and return from functions
let complexFunction1 f x = f(x)

// just an example of returning functions...
let complexFunction2 f g x = fun y -> g(f x y)  

// as an aside, here's how the above line of code would have to be written in a not-so-functional language such as C#:
(*
public static Func<T2, TResult> ComplexFunction2<T1, T2, TResult>(Func<T1, T2, TResult> f, Func<T1, T2> g, T1 x)
    {
        return y => g(f(x, y));
    }

// C# doesn't have a direct way of representing functions so it must use a construct called a delegate.
// Also C# requires the generic types to be written explicitly
// Not fun.
*)


// how to pass a function to a function. One option is to 
// define it directly as in the helper function below:
let squareOf3 = 

    // let's define a helper function
    let helper x = x * x
    // this helper function can be passed as a value
    complexFunction1 helper 3
    
// same thing, but the funcion is defined inline, anonymously
let squareOf4 = complexFunction1 (fun x -> x * x) 3
    

/////////////////////////////////////
// Record types and union types
/////////////////////////////////////

// records are simply a way to group simple values into complex ones
// F# records are great because they can be defined and created succintly
// they're immutable, they define equality as you expect it to work from a human, common sense point of view
// and the compiler will give you an error unless all values are supplied when creating values

type SimpleHouse = 
    { NumberOfRooms: int
      Price: float }

// create a few record types and you can model things quite effectively with little effort
type Address = 
    { Line1: string 
      Line2: string
      City: string
      PostCode: string }

// let's do better now
type House = 
    { NumberOfRooms: int 
      Price: float
      Address: Address }

// let's create a value of type House

let sampleHouse: House = 
    { NumberOfRooms = 3
      Price = 435000.
      Address = { Line1 = "some st."; Line2 = "..."; City = "Manchester"; PostCode = "M4 EF3" } }

// the type of the Address property was inferred
// the type of sampleHouse was annotated explicitly 

// F# also offers union types.
// better seen with an example
type Property = 
    | House 
    | Flat

// a Property can be either a House or a Flat
// and these "choices" can have data attached to them

type Flat = 
    { NumberOfRooms: int 
      FloorLevel: int
      HasBalcony: bool}

type Property2 = 
    | House of House
    | Flat of Flat 

// it's common to use pattern matching to extract values out of these union types
// this also shows the use of dot-notation to access record members
let numberOfRooms property = 
    match property with
    | Property2.House house -> house.NumberOfRooms
    | Property2.Flat flat -> flat.NumberOfRooms


/////////////////////////////////////
// Collections: lists, arrays, maps
/////////////////////////////////////

// lists: an immutable collection of ordered values. The most common F# collection
let list1 = [ 1; 3; 5 ]

// using indentation to limit scope: silly example
let list2 = 
    let first = 2
    let second = 99
    [ first; second ]


// value of list2 at this point is [2; 99]
// the 'first' and 'second' values are not available outside the definition of list2
// creating scopes or blocks of code in this way prevents unnecessary pollution of global scopes

// arrays are defined similarly to lists

let array1 = [| "one"; "two" |]

// maps are immutable dictionaries: in other words, a collection of key-value pairs
// they're easily defined as a list of tuples where the first element of the tuple is the key
// and the second is the value. This list then is passed as a value to the Map.ofSeq function 
// to construct the map

let salesByMonth =  // builds a Map<string, int>, a dictionary where the key is a string and the value is an int
    Map.ofSeq (
        [ "Jan", 935
          "Feb", 433
          "Mar", 675
          "Apr", 100]
    )

// The |> operator: Whatever is to the left of the operator will be passed as the last argument of the function placed to the right of the operator

let sum x y = x + y

let value1 = sum 3 5 // normal way to invoke the function
let value2 = 5 |> sum 3 // equivalent to the above

// why use this operator? because it makes chaining operations very easy
let salesByMonth2 = [ ("Jan", 935); ("Feb", 433); ("Mar", 675); ("Apr", 100) ] |> Map.ofSeq // another way to define the salesByMonth dictionary

// that's of course just one function call. Where the |> operator really makes sense is when multiple function calls are involved

let salesLowerThan500 = 
    salesByMonth2
    |> Map.toList
    |> List.filter(fun (monthName, monthSales) -> monthSales < 500)
    |> List.map (fun (monthName, monthSales) -> monthSales) // returns [433; 100], that is, a list containing all values lower than 500

// the above will chain three function calls. First the map is converted back to a list of tuples
// then the filter function selects those tuples where the value is lower than 500
// finally the map function discards the name of the month and returns just the value
// this is essentially a succint way of buildig a pipeline of transformations, a key technique in functional programming

// combining sequences using the yield and yield! keywords
// yield adds an element to a sequence
// yield! adds a whole sequence to a sequence

let list3 = [ 4; 5; 6]
let list4 = 
    [ 
        yield 1 
        yield 2
        yield 3
        yield! list3
        yield 7
    ]
    
// the result: a list containing [1; 2; 3; 4; 5; 6; 7]
