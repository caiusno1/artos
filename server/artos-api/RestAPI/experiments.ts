import { FindConditions } from 'typeorm';
import { DB_Token } from './../DataStructure/DB_Token';
import { DB_Author } from './../DataStructure/DB_Author';
import { DB_Experiment } from './../DataStructure/DB_Experiment';
import {Router} from 'express'
import { DataBaseService } from '../DataStructure/DataBaseService';
import { returnOnFailure } from '../helpers/returnOnFailure';
import passport from 'passport';
import config from '../config/config.json'
import { TokenExpiredError } from 'jsonwebtoken';

const experimentsRouter = Router()


experimentsRouter.get('/', passport.authenticate(['token', 'jwt'], {session: false}) ,  async function(req, res, next) {
    const dbServ = await DataBaseService.getInstance()
    if(!returnOnFailure(req.user,req,res)){return}
    // A simple shortCut to terminate the connection when the first paramter is falsy
    if(!returnOnFailure(dbServ,req,res)){return}
    const artosExperimentRepo = dbServ.connection.getRepository(DB_Experiment)
    if(!returnOnFailure(artosExperimentRepo,req,res)){return}
    const authorRepo = dbServ.connection.getRepository(DB_Author)
    if(!returnOnFailure(authorRepo,req,res)){return}
    let artosResults;
    if((req.user as any).user){
        const currentAuthor = await authorRepo.findOne({username:(req.user as any)?.user?.username})
        if(!returnOnFailure(currentAuthor,req,res)){return}
        artosResults = await artosExperimentRepo.find({author:currentAuthor})
    }
    else if ((req.user as any).token){
        // TODO implement more efficient
        artosResults = []
        const artosTokenRepo = await dbServ.connection.getRepository(DB_Token)
        for(const experiment of await artosExperimentRepo.find({relations:["tokens"]}))
        {
            if(experiment.tokens.find(((xp) => xp.Token == (req.user as any).token.Token))){
                artosResults.push(experiment)
            }
        }
    }
    else {
        if(!returnOnFailure(undefined,req,res)){return}
    }
    
    
    // A simple shortCut to terminate the connection when the first paramter is falsy
    if(!returnOnFailure(artosResults,req,res)){return}
    res.send(artosResults)
});


experimentsRouter.get('/:resultId', passport.authenticate(['token', 'jwt'], {session: false}) ,  function(req, res, next) {

});

experimentsRouter.post('/', passport.authenticate(['token', 'jwt'], {session: false}) ,  function(req, res, next) {

});

experimentsRouter.delete('/:xpID', passport.authenticate(['token', 'jwt'], {session: false}) ,  async function(req, res, next) {
    const dbServ = await DataBaseService.getInstance()
    // A simple shortCut to terminate the connection when the first paramter is falsy
    if(!returnOnFailure(dbServ,req,res)){return}
    if(!returnOnFailure(req.user,req,res)){return}
    if(!returnOnFailure((req.user as any)?.user,req,res)){return}
    if(!returnOnFailure((req.user as any)?.user?.username,req,res)){return}
    const authorRepo = await dbServ.connection.getRepository(DB_Author)
    if(!returnOnFailure(authorRepo,req,res)){return}
    const XPRepo = await dbServ.connection.getRepository(DB_Experiment)
    if(!returnOnFailure(XPRepo,req,res)){return}
    const tokenRepo = await dbServ.connection.getRepository(DB_Token)
    if(!returnOnFailure(tokenRepo,req,res)){return}
    const currentAuthor = await authorRepo.findOne({username:(req.user as any)?.user?.username})
    if(currentAuthor){
        if(!returnOnFailure(Number.parseInt(req.params.xpID),req,res)){return}
        const XP2delete = await XPRepo.findOne({ID:Number.parseInt(req.params.xpID), author:currentAuthor})
        if(XP2delete){        
            XPRepo.delete(XP2delete);
            for(const token of await tokenRepo.find({ experiment: XP2delete })){
                tokenRepo.delete(token)
            }

        }
        res.send({status:"ok"})
    }
    else {
        res.send({status:"not possible"})
    }
});

experimentsRouter.put('/', passport.authenticate(['token', 'jwt'], {session: false}) ,  async function(req, res, next) {
    const dbServ = await DataBaseService.getInstance()
    // A simple shortCut to terminate the connection when the first paramter is falsy
    if(!returnOnFailure(dbServ,req,res)){return}
    if(!returnOnFailure(req.user,req,res)){return}
    const experiment = new DB_Experiment()
    const authorRepo = await dbServ.connection.getRepository(DB_Author)
    if(!returnOnFailure((req.user as any)?.user,req,res)){return}
    if(!returnOnFailure((req.user as any)?.user?.username,req,res)){return}
    const tokenRepo = await dbServ.connection.getRepository(DB_Token)
    if(!returnOnFailure(tokenRepo,req,res)){return}
    const currentAuthor = await authorRepo.findOne({username:(req.user as any)?.user?.username})
    if(currentAuthor){
        experiment.author = currentAuthor;
        const initToken = new DB_Token()
        initToken.Token = config.passkey
        const ctoken = await tokenRepo.save(initToken)
        experiment.tokens = [ctoken]
        const repo = dbServ.connection.getRepository(DB_Experiment)
        await repo.save(experiment)
        res.send({status:"ok"})
    }
    else {
        res.send({status:"not possible"})
    }

});

export {experimentsRouter}