import { IResult } from './IResult';
import { Entity, PrimaryGeneratedColumn, Column, PrimaryColumn, ManyToOne, JoinColumn, ManyToMany, OneToMany } from "typeorm";
import { DB_Experiment } from './DB_Experiment';

@Entity()
export class DB_Author {
    @PrimaryColumn()
    public username!: string
    @Column()
    public Name!: string;
    @Column()
    public password!: string
    @Column()
    public salt!: string
    @OneToMany(() => DB_Experiment, experiment => experiment.author)
    public experiments!: DB_Experiment[]

}