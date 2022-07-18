import { DB_Participant } from './DB_Participant';
import { DB_Experiment } from './DB_Experiment';
import { Connection, createConnection } from "typeorm";
import { DB_ARTOS_Error } from "./DB_ARTOS_Error";
import { DB_Result } from "./DB_Result";
import { DB_Token } from './DB_Token';
import { DB_Author } from './DB_Author';

const DB_TYPE = ((process.env.DB_TYPE) ? process.env.DB_TYPE : "mysql") as "mysql"|"mariadb"
const DB_HOST = ((process.env.DB_HOST) ? process.env.DB_HOST : "localhost") as string
const DB_PORT = (process.env.DB_PORT && !Number.isNaN(process.env.DB_PORT)) ? Number.parseInt(process.env.DB_PORT) : 3306
const DB_USER = (process.env.DB_USER) ? process.env.DB_USER : "root"
const DB_PASSWD = process.env.DB_PASSWD ? process.env.DB_PASSWD : "root"
const DB_NAME = process.env.DB_NAME ? process.env.DB_NAME : "artosdb"

export class DataBaseService{
    connection!: Connection
    private static instance: DataBaseService|undefined
    private initialize():Promise<DataBaseService>{
        DataBaseService.instance = this
        return new Promise( (res,rej) => {
            const connectionPromis = createConnection({
                type: DB_TYPE,
                host: DB_HOST,
                port: DB_PORT,
                username: DB_USER,
                password: DB_PASSWD,
                database: DB_NAME,
                entities: [
                    DB_Result,
                    DB_ARTOS_Error,
                    DB_Experiment,
                    DB_Participant,
                    DB_Token,
                    DB_Author,
                ],
                synchronize: true,
                logging: false
            })
            connectionPromis.then(connection => {
                this.connection = connection
                res(DataBaseService.instance as DataBaseService)
            }).catch( (res) =>rej(res))
        })
    }
    static getInstance():Promise<DataBaseService>
    {
        if(this.instance)
        {
            return new Promise( (res) => {
                res(this.instance as DataBaseService)
            })
        }
        else
        {
            this.instance = new DataBaseService()
            
            return this.instance.initialize()
        }
    }
}