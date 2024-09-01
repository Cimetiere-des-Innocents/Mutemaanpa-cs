namespace Kernel

(* 
Timer:
    record how many times has passed since last tick.
*)

[<CLIMutable>]
[<Struct>]
type Timer =
    { mutable elapsed: double
      mutable accumulated: double
      threshold: double }

    member self.reset() = self.elapsed <- 0

    (* Tick returns true if the time reaches threshold, false otherwise. *)
    member self.tick elapsed =
        self.accumulated <- self.accumulated + elapsed
        let newElapsed = self.elapsed + elapsed

        if newElapsed + 1e-9 >= self.threshold then
            self.reset ()
            true
        else
            self.elapsed <- newElapsed
            false

module Timer =

    let newTimer threshold =
        { elapsed = 0
          accumulated = 0
          threshold = threshold }
