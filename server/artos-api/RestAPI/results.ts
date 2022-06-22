import { DB_Experiment } from './../DataStructure/DB_Experiment';
import { DB_Result } from './../DataStructure/DB_Result';
import {Router} from 'express'
import passport from 'passport';
import { DataBaseService } from '../DataStructure/DataBaseService';
import { IResult } from '../DataStructure/IResult';
import { returnOnFailure } from '../helpers/returnOnFailure';

const resultsRouter = Router()

let tmpStore:IResult[][] = []

function resultEquals(a:IResult, b: IResult){
    return a.participant_id === b.participant_id && a.timestamp === b.timestamp && a.result === b.result && a.experiment === b.experiment
}


resultsRouter.get('/', passport.authenticate(['token', 'jwt'], {session: false}), async function(req, res) {
    const dbServ = await DataBaseService.getInstance()
    // A simple shortCut to terminate the connection when the first paramter is falsy
    if(!returnOnFailure(dbServ,req,res)){return}
    const artosResultRepo = dbServ.connection.getRepository(DB_Result)
    const artosResults = await artosResultRepo.find()
    // A simple shortCut to terminate the connection when the first paramter is falsy
    if(!returnOnFailure(artosResults,req,res)){return}
    res.send(artosResults)
});


resultsRouter.get('/:resultId',passport.authenticate(['token', 'jwt'], {session: false}), function(req, res) {

});

resultsRouter.post('/',passport.authenticate(['token', 'jwt'], {session: false}), function(req, res) {
    
});

resultsRouter.delete('/',passport.authenticate(['token', 'jwt'], {session: false}), async function(req, res) {
    const dbServ = await DataBaseService.getInstance()
    // A simple shortCut to terminate the connection when the first paramter is falsy
    if(!returnOnFailure(dbServ,req,res)){return}
    if(!returnOnFailure(req.body,req,res)){return}
    const artosResultRepo = dbServ.connection.getRepository(DB_Result)
    const artosResults = await artosResultRepo.remove(req.body)
    res.send({status:"ok"})
});

// Only put works for unity upload https://forum.unity.com/threads/posting-json-through-unitywebrequest.476254/
resultsRouter.put('/', passport.authenticate(['token', 'jwt']), async function(req, res) {
    const dbServ = await DataBaseService.getInstance()
    // A simple shortCut to terminate the connection when the first paramter is falsy
    if(!returnOnFailure(dbServ,req,res)){return}
    if(!returnOnFailure(req.body,req,res)){return}
    const artosResultRepo = dbServ.connection.getRepository(DB_Result)
    if(!returnOnFailure(artosResultRepo,req,res)){return}
    const artosExperimemtRepo = dbServ.connection.getRepository(DB_Experiment)
    if(!returnOnFailure(artosExperimemtRepo,req,res)){return}
    const artosResults = await artosResultRepo.find({participant_id:req.body[0].id, timestamp:req.body[0].id})
    for(let result of req.body){
        if(artosResults.filter((sRes) => resultEquals(result,sRes)).length === 0 )
        {
            const id = (result as DB_Result).experiment.ID
            const experiment = await artosExperimemtRepo.findOne({ID:id});
            if(!returnOnFailure(experiment,req,res)){return}
            result.experiment = experiment;
            artosResultRepo.save(result);
        }
    }
    // A simple shortCut to terminate the connection when the first paramter is falsy
    if(!returnOnFailure(artosResults,req,res)){return}
    res.send(artosResults)
});

export {resultsRouter}