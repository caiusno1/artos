import { DB_Author } from './DB_Author';
import { DB_Token } from './DB_Token';
import { Column, Entity, JoinTable, ManyToOne, OneToMany, PrimaryGeneratedColumn } from "typeorm";

@Entity()
export class DB_Experiment{
    @PrimaryGeneratedColumn('increment')
    public ID!: number;
    @ManyToOne(() => DB_Author, author => author.experiments)
    public author!: DB_Author
    @OneToMany(() => DB_Token, token => token.experiment, { onDelete: 'CASCADE' })
    @JoinTable()
    public tokens!: DB_Token[]
}