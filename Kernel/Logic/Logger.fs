namespace Kernel

(*
Logger:
    Helps us to observe what event happened inside our system.
    Global static object.

    Default output is through stdout.
    Using Godot output:
    ```
    Logger.setLogger GD.Print
    ```
*)
module Logger =
    open System

    let mutable private registeredLogger: Action<string> =
        new Action<string>(Console.WriteLine)

    let info s = registeredLogger.Invoke $"[INFO] {s}"

    let warn s = registeredLogger.Invoke $"[WARN] {s}"

    let setLogger logger = registeredLogger <- logger
