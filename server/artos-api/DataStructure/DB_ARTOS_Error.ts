import { Entity, PrimaryGeneratedColumn, Column } from "typeorm";
import { IArtosError } from "./IArtosError";

@Entity()
export class DB_ARTOS_Error implements IArtosError{
    [x: string]: any;

    @PrimaryGeneratedColumn()
    public id!: number;
    @Column()
    public message!: string;
    @Column()
    public errorID!: number;

}