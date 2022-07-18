import { IResult } from './IResult';
import { Entity, PrimaryGeneratedColumn, Column, PrimaryColumn, ManyToOne, JoinColumn } from "typeorm";
import { DB_Experiment } from './DB_Experiment';

@Entity()
export class DB_Token {

    @PrimaryGeneratedColumn('increment')
    public ID!: number;
    @Column()
    public Token!: string;
    @ManyToOne(() => DB_Experiment, experiment => experiment.tokens)
    public experiment!: DB_Experiment

}