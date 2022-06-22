import { DB_ARTOS_Error } from './../DataStructure/DB_ARTOS_Error';
import { DataBaseService } from './../DataStructure/DataBaseService';
import {Router} from 'express'
import { Request, Response } from 'express-serve-static-core';
import { ParsedQs } from 'qs';
import { returnOnFailure } from '../helpers/returnOnFailure';
import { FindConditions } from 'typeorm';

const errorsRouter = Router()

errorsRouter.get('/', async function(req, res) {
    console.log("get no param")
    const dbServ = await DataBaseService.getInstance()
    // A simple shortCut to terminate the connection when the first paramter is falsy
    if(!returnOnFailure(dbServ,req,res)){return}
    const artosErrorRepo = dbServ.connection.getRepository(DB_ARTOS_Error)
    const artosErrors = await artosErrorRepo.find()
    // A simple shortCut to terminate the connection when the first paramter is falsy
    if(!returnOnFailure(artosErrors,req,res)){return}
    res.send(artosErrors)
});


errorsRouter.get('/:errorId', async function(req, res) {
    const dbServ = await DataBaseService.getInstance()
    // A simple shortCut to terminate the connection when the first paramter is falsy
    if(!returnOnFailure(dbServ,req,res)){return}
    const artosErrorRepo = dbServ.connection.getRepository(DB_ARTOS_Error)
    // A simple shortCut to terminate the connection when the first paramter is falsy
    if(!returnOnFailure(!Number.isNaN(Number.parseInt(req.params.errorId)), req,res)){return}
    // A simple shortCut to terminate the connection when the first paramter is falsy
    const artosErrors = await artosErrorRepo.find({id:Number.parseInt(req.params.errorId)})
    if(!returnOnFailure(artosErrors,req,res)){return}
    res.send(artosErrors)
});

errorsRouter.post('/', function(req, res) {

});

errorsRouter.delete('/:errorId', async function(req, res) {
    const dbServ = await DataBaseService.getInstance()
    // A simple shortCut to terminate the connection when the first paramter is falsy
    if(!returnOnFailure(dbServ,req,res)){return}
    const repo = dbServ.connection.getRepository(DB_ARTOS_Error)
    if(!returnOnFailure(!Number.isNaN(Number.parseInt(req.params.errorId)), req,res)){return}
    const error2delete = await repo.findOne({ id: Number.parseInt(req.params.errorId)});
    if(error2delete){
        await repo.remove(error2delete)
    }
    res.send({status:"ok"})

});
// Only Put works for unity upload ?! https://forum.unity.com/threads/posting-json-through-unitywebrequest.476254/
errorsRouter.put('/', async function(req, res) {
    const dbServ = await DataBaseService.getInstance()
    // A simple shortCut to terminate the connection when the first paramter is falsy
    if(!returnOnFailure(dbServ,req,res)){return}
    const error = new DB_ARTOS_Error()
    error.errorID = req.body.errorID
    error.message = req.body.message
    const repo = dbServ.connection.getRepository(DB_ARTOS_Error)
    await repo.save(error)
    res.send({status:"ok"})
});

export {errorsRouter}