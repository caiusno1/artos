import { DB_Experiment } from './../DataStructure/DB_Experiment';
import { DB_Participant } from '../DataStructure/DB_Participant';
import {Router} from 'express'
import { DataBaseService } from '../DataStructure/DataBaseService';
import { returnOnFailure } from '../helpers/returnOnFailure';
import passport from 'passport';

const participantsRouter = Router()

participantsRouter.get('/:experimentID', passport.authenticate(['token', 'jwt'], {session: false}), async function(req, res, next) {
    const dbServ = await DataBaseService.getInstance()
    if(!returnOnFailure(dbServ,req,res)){return}
    if(!returnOnFailure(Number.parseInt(req.params.experimentID),req,res)){return}
    const artosResultRepo = dbServ.connection.getRepository(DB_Participant)
    const results = await artosResultRepo.find({experiment:{ID:(Number.parseInt(req.params.experimentID))},isCurrent:true})
    const artosExRepo = dbServ.connection.getRepository(DB_Experiment)
    const currentExp = await artosExRepo.findOne({ID:Number.parseInt(req.params.experimentID)})
    if(!returnOnFailure(currentExp,req,res)){return}
    if(!currentExp) res.send()
    else
    if(results.length === 0){
        results.push(new DB_Participant())
        results[0].isCurrent = true
        results[0].name = "undefined";
        results[0].experiment = currentExp;
        await artosResultRepo.save(results)
        res.send(results[0])
    }
    else if (results.length > 1){
        console.log(results)
        const dummy = new DB_Participant()
        dummy.isCurrent = true
        dummy.name = "dummy";
        console.log("Dummy started ... Something is complete broke here...")
        res.send(dummy)
    }
    else {
        if(results.length !== 1){
            console.log("What happen here .... Length below zero")
            const dummy = new DB_Participant()
            dummy.isCurrent = true
            dummy.name = "dummy";
            console.log("Dummy started ...")
            res.send(dummy)
        }
        else
        {
            res.send(results[0])
        }
    }
});

participantsRouter.post('/:experimentID', passport.authenticate(['token', 'jwt'], {session: false}), async function(req, res, next) {
    if(!returnOnFailure(Number.parseInt(req.params.experimentID),req,res)){return}
    const participant: DB_Participant = req.body
    if(participant != null)
    {
        const dbServ = await DataBaseService.getInstance()
        if(!returnOnFailure(dbServ,req,res)){return}
        const artosResultRepo = dbServ.connection.getRepository(DB_Participant);
        const participants = (await artosResultRepo.find({experiment:{ID:(Number.parseInt(req.params.experimentID))},isCurrent:true}))
        for(const cparticipant of participants){
            if(cparticipant.ID !== participant.ID){
                cparticipant.isCurrent = false;
            }
        }
        await artosResultRepo.save(participants)
        const partipantIfExisting = await artosResultRepo.findOne(participant)
        if(partipantIfExisting){
            partipantIfExisting.isCurrent = true
            await artosResultRepo.save(partipantIfExisting)
        }
        else {
            participant.isCurrent = true
            const artosEXRepo = dbServ.connection.getRepository(DB_Experiment);
            const exp = await artosEXRepo.findOne({ID:participant.experiment.ID})
            if(exp){
                participant.experiment = exp;
                await artosResultRepo.save(participant)
            }
        }
        res.send({status:"success"})
    }
    else
    {
        res.send({status:"error"})
    }
});
export {participantsRouter}