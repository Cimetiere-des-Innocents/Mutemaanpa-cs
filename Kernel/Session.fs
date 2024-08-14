namespace Kernel

type Session = {
    position: Position.Store
}

module Session =
    let bootstrap = ()
    let load savePath = {
        position = Position.Store savePath
    }
    
    let tick elapsed = elapsed

    let a = 1
