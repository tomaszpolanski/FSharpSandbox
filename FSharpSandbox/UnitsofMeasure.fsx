
[<Measure>] type s

let timeout = 1<s>  

let sleepThread (time : int<s> ) = 
    System.Threading.Thread.Sleep( (int)time)


sleepThread 1<s>