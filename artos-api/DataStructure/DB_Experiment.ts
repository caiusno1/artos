import { Column, Entity, PrimaryGeneratedColumn } from "typeorm";

@Entity()
export class DB_Experiment{
    @PrimaryGeneratedColumn('increment')
    public ID!: number;
    @Column()
    public author!: string
}