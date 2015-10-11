
[<Measure>] type s

let timeout = 1<s>  

let sleepThread (time : int<s>) = 
    System.Threading.Thread.Sleep( (int)time)


sleepThread 1<s>

[<Measure>] type m

let speed = 20<m/s>

let distance = 10<m>

let anotherSpeed = distance / timeout