import { DB_Experiment } from './DB_Experiment';
export interface IResult{
    timestamp: number
    result: string
    participant_id: string
    experiment: DB_Experiment
}