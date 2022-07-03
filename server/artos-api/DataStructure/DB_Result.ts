import { IResult } from './IResult';
import { Entity, PrimaryGeneratedColumn, Column, PrimaryColumn, ManyToOne, JoinColumn } from "typeorm";
import { DB_Experiment } from './DB_Experiment';

@Entity()
export class DB_Result implements IResult{

    @PrimaryGeneratedColumn('increment')
    public ID!: number;
    @PrimaryColumn()
    public trialUID!: number;
    @Column()
    public timestamp!: number;
    @Column("longtext")
    public result!: string;
    @Column()
    public participant_id!: string;
    @ManyToOne(type => DB_Experiment)
    public experiment!: DB_Experiment

}