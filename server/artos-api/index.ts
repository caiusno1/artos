import { DB_Author } from './DataStructure/DB_Author';
import { DataBaseService } from './DataStructure/DataBaseService';
import { DB_ARTOS_Error } from './DataStructure/DB_ARTOS_Error';
import express from 'express';
import "reflect-metadata";
import { errorsRouter } from './RestAPI/errors';
import { experimentsRouter } from './RestAPI/experiments';
import { participantsRouter } from './RestAPI/participant';
import { resultsRouter } from './RestAPI/results';
import * as config from "./config/config.json"
import passport from 'passport'
import { configurePassport } from './helpers/configurePassport';
import cors from 'cors';
import { jupyterRouter } from './jupyterAPI/jupyter';
import bcrypt from 'bcryptjs';

// rest of the code remains same
const errors: string[] = []
const app = express();
const PORT = 8000;

/*
 * To be able to the proxy method for the web editor request,
 * we have to enable Cross-Origin Resource Sharing (CORS).
 * What is CORS: https://developer.mozilla.org/en-US/docs/Web/HTTP/CORS
 */
app.use(cors())

//CORS Middleware
//app.use(cors())

//app.options('*', cors)
app.use(express.json({limit: '1gb'}));
app.use(express.urlencoded({limit: '1gb', extended: true }));


DataBaseService.getInstance().then(async dbServ => {
    configurePassport(passport,app,config, dbServ)
    const repository = dbServ.connection.getRepository(DB_Author);
    const singleUser = await repository.findOne()

    if(!singleUser){
        var salt = bcrypt.genSaltSync(10);
        var hash = bcrypt.hashSync(config.adminPassword, salt);
        const admin = new DB_Author()
        admin.Name = "Admin"
        admin.username = "admin"
        admin.salt = salt
        admin.password = hash

        var salt = bcrypt.genSaltSync(10);
        var hash = bcrypt.hashSync(config.adminPassword, salt);

        const user = new DB_Author()
        user.Name = "User"
        user.username = "user"
        user.salt = salt
        user.password = hash

        repository.save([admin, user])
    }

    // default routes
    app.get('/', (req, res) => res.send('Artos API server'));
    // loggin routes
    app.use('/errors', passport.authenticate(['token', 'jwt'], {session: false}) , errorsRouter)
    // experiment routes
    app.use('/experiments', experimentsRouter)
    // participant routes
    app.use('/participant', participantsRouter)
    // result routes
    app.use('/results', resultsRouter)
     // jupyter route
     app.use('/jupyter', jupyterRouter)

    app.listen(PORT, () => {
        console.log(`⚡️[server]: Server is running at https://localhost:${PORT}`);
    });
}).catch((error:any) => console.log(error));
