namespace Kernel

module Logger =
    open System

    type Logger(endpoint) =
        member _.info s = endpoint $"[INFO] {s}"
        member _.warn s = endpoint $"[WARN] {s}"

    let private defaultLogger = Logger(Console.WriteLine)

    let mutable private registeredLogger = None

    let setLogger logger = registeredLogger <- logger

    let logger () =
        match registeredLogger with
        | None -> defaultLogger
        | Some l -> l
