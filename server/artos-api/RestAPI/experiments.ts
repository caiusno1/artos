import { DB_Experiment } from './../DataStructure/DB_Experiment';
import {Router} from 'express'
import { DataBaseService } from '../DataStructure/DataBaseService';
import { returnOnFailure } from '../helpers/returnOnFailure';
import passport from 'passport';

const experimentsRouter = Router()


experimentsRouter.get('/', passport.authenticate(['token', 'jwt'], {session: false}) ,  async function(req, res, next) {
    const dbServ = await DataBaseService.getInstance()
    // A simple shortCut to terminate the connection when the first paramter is falsy
    if(!returnOnFailure(dbServ,req,res)){return}
    const artosResultRepo = dbServ.connection.getRepository(DB_Experiment)
    const artosResults = await artosResultRepo.find()
    // A simple shortCut to terminate the connection when the first paramter is falsy
    if(!returnOnFailure(artosResults,req,res)){return}
    res.send(artosResults)
});


experimentsRouter.get('/:resultId', passport.authenticate(['token', 'jwt'], {session: false}) ,  function(req, res, next) {

});

experimentsRouter.post('/', passport.authenticate(['token', 'jwt'], {session: false}) ,  function(req, res, next) {

});

experimentsRouter.delete('/:resultId', passport.authenticate(['token', 'jwt'], {session: false}) ,  function(req, res, next) {

});

experimentsRouter.put('/', passport.authenticate(['token', 'jwt'], {session: false}) ,  async function(req, res, next) {
    const dbServ = await DataBaseService.getInstance()
    // A simple shortCut to terminate the connection when the first paramter is falsy
    if(!returnOnFailure(dbServ,req,res)){return}
    const experiment = new DB_Experiment()
    experiment.author = "kai";
    const repo = dbServ.connection.getRepository(DB_Experiment)
    await repo.save(experiment)
    res.send({status:"ok"})
});

export {experimentsRouter}