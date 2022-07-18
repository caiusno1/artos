import { DB_Experiment } from './DB_Experiment';
import { Column, Entity, ManyToOne, PrimaryColumn, PrimaryGeneratedColumn } from "typeorm";

@Entity()
export class DB_Participant{
    @ManyToOne(type => DB_Experiment, { onDelete: 'CASCADE' })
    public experiment!: DB_Experiment;
    @PrimaryGeneratedColumn('increment')
    public ID!: number;
    @PrimaryColumn()
    public name!: string;
    @Column()
    public isCurrent!: boolean;
}