namespace Kernel

module Logger =
    open System

    let mutable private registeredLogger: Action<string> =
        new Action<string>(Console.WriteLine)

    let info s = registeredLogger.Invoke $"[INFO] {s}"

    let warn s = registeredLogger.Invoke $"[WARN] {s}"

    let setLogger logger = registeredLogger <- logger
