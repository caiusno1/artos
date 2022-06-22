export function returnOnFailure(param:any, req: any, res: { send: (arg0: string) => void; }): boolean{
    if(!param){
        res.send("Error")
        return false;
    }
    return true
}