import { DB_Participant } from '../DataStructure/DB_Participant';
import {Router} from 'express'
import { DataBaseService } from '../DataStructure/DataBaseService';
import { returnOnFailure } from '../helpers/returnOnFailure';
import passport from 'passport';

const participantsRouter = Router()

participantsRouter.get('/', passport.authenticate(['token', 'jwt'], {session: false}), async function(req, res, next) {
    const dbServ = await DataBaseService.getInstance()
    if(!returnOnFailure(dbServ,req,res)){return}
    const artosResultRepo = dbServ.connection.getRepository(DB_Participant)
    const results = await artosResultRepo.find({isCurrent:true})
    if(results.length === 0){
        results.push(new DB_Participant())
        results[0].isCurrent = true
        results[0].name = "undefined";
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

participantsRouter.post('/', passport.authenticate(['token', 'jwt'], {session: false}), async function(req, res, next) {
    const participant: DB_Participant = req.body
    if(participant != null)
    {
        const dbServ = await DataBaseService.getInstance()
        if(!returnOnFailure(dbServ,req,res)){return}
        const artosResultRepo = dbServ.connection.getRepository(DB_Participant);
        const participants = (await artosResultRepo.find({isCurrent:true}))
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
            await artosResultRepo.save(participant)
        }
        res.send("success")
    }
    else
    {
        res.send("error")
    }
});
export {participantsRouter}