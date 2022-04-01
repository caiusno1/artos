import { Express } from "express-serve-static-core";
import { PassportStatic } from "passport";
import {ExtractJwt, Strategy as JWTStrategy} from 'passport-jwt'
import {Strategy as LocalStrategy} from 'passport-local'

import * as jwt from 'jsonwebtoken'

import { UniqueTokenStrategy } from 'passport-unique-token';

export function configurePassport(passport:PassportStatic, app: Express, config: any): void{

    // Do not know whether still necessary but will not hurt
    app.set('trust proxy', 'loopback')

    // initialize passport for usage as authentification management
    app.use(passport.initialize());

    passport.serializeUser(function(user, done) {
        if(user) done(null, user);
    });
    
    passport.deserializeUser(function(id:any, done) {
        done(null, id);
    });

    passport.use(
        new UniqueTokenStrategy({tokenQuery: 'code'},(token, done) => {
        if(token === config.passkey){
            done(null,{user:"kai"})
        }
        else {
            done(null,false)
        }
        }),
    );

    const strategy = new LocalStrategy(
        function(username, password, done) {
            Promise.resolve((username === "kai" && password === config.adminPassword)?{name:"kai", id:0}:undefined)
            .then((user) => {
                if(user){
                    return done(null, {name:user.name, id:0});
                }
                else {
                    return done("unauthorized access", false);
                }
            })
            .catch((err) =>{
                return done("unauthorized access", false);
            });
        }
    );
    passport.use(strategy);
    passport.use(new JWTStrategy({
        jwtFromRequest: ExtractJwt.fromAuthHeaderAsBearerToken(),
        // hardcoded JWT Auth Token encryption key 
        // TODO replace with configurable variable
        secretOrKey   : config['jwt-secret']
    },
    function (jwtPayload, cb) {
        // find the user in db.
        console.log(jwtPayload)
        if(jwtPayload.name === "kai"){
            return cb(null,jwtPayload);
        }
    }
    ));

    app.post('/authenticate', function (req, res, next) { passport.authenticate('local', {session: false}, (err, user, info) => {
        if (err || !user) {
            return res.status(400).json({
                message: `Something is not right ${err}`,
                user   : user
            });
        }       
        req.login(user, {session: false}, (err) => {
           if (err) {
               res.send(err);
           }           // generate a signed son web token with the contents of user object and return it in the response           
           const token = jwt.sign(user, config['jwt-secret']);
           return res.json({user, token});
        });
    })(req, res);
    });

}