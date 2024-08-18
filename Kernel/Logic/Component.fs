namespace Kernel

open System

[<Struct>]
[<CLIMutable>] // to make dapper work
type Vector3 = { X: float; Y: float; Z: float }

module Position =
    let a = 1
    