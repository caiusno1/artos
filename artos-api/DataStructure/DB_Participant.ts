import { Column, Entity, PrimaryGeneratedColumn } from "typeorm";

@Entity()
export class DB_Participant{
    @PrimaryGeneratedColumn('increment')
    public ID!: number;
    @Column()
    public name!: string;
    @Column()
    public isCurrent!: boolean;
}