import { Router } from "express";
import { exec } from "child_process";
import { existsSync, mkdirSync, writeFileSync} from "fs";
import { DataBaseService } from "../DataStructure/DataBaseService";
import { returnOnFailure } from "../helpers/returnOnFailure";
import { DB_Result } from "../DataStructure/DB_Result";
import { json2csv } from "json-2-csv";
import { jupyterdataLoaderTemplate } from "./jupyterdataLoaderTemplate";


const jupyterRouter = Router()

jupyterRouter.put('/', async function(req, res) {
    const ID = req.body.expermimentID
    const participantID = req.body.participantID
    const conditionIDs = req.body.conditionIDs
    const dbServ = await DataBaseService.getInstance()
    if(!returnOnFailure(dbServ,req,res)){return}
    if(Number.isNaN(Number.parseInt(ID))){
      res.send("error")
      return
    }
    const artosResultRepo = dbServ.connection.getRepository(DB_Result)
    const results = await artosResultRepo.find({experiment:ID})
    const data :string = await new Promise((res,rej) => {
      json2csv(results, (err,csv) => {
        if(err) rej("")
        else if(!csv) res("")
        else if(csv) res(csv)
      })
    })
    console.log(data)
    const dl_lines = jupyterdataLoaderTemplate.dataloader.split("\n").map((line:string) => '"'+line+'\\n"')
    try {
        const folderAsList = __dirname.split("/")
        folderAsList.pop();
        const folder = folderAsList.join("/")  
        if(!existsSync(folder+"/jupyter/experiments")){
          mkdirSync(folder+"/jupyter/experiments");
        }
        if(!existsSync(folder+`/jupyter/experiments/experiment${ID}/`)){
          mkdirSync(folder+`/jupyter/experiments/experiment${ID}/`);
        }

        writeFileSync(folder+`/jupyter/experiments/experiment${ID}/data.csv`,data)
        writeFileSync(folder+`/jupyter/experiments/experiment${ID}/data.json`,JSON.stringify(results))
        writeFileSync(folder+`/jupyter/experiments/experiment${ID}/notebook.ipynb`,`{
          "cells": [
           {
            "cell_type": "markdown",
            "metadata": {},
            "source": [
             "## Hello World"
            ]
           },
           {
            "cell_type": "code",
            "execution_count": null,
            "metadata": {},
            "outputs": [],
            "source": [
             "import pandas as pd; \\n",
             "import pymc as pm; import aesara.tensor as at; import arviz as az; \\n",
             "from aesara.tensor import cast\\n",
             "import numpy as np; \\n",
             "import matplotlib.pyplot as plt"
            ]
           },
           {
            "cell_type": "code",
            "execution_count": null,
            "metadata": {},
            "outputs": [],
            "source": [
             ${dl_lines.join(",\n")}
            ]
           },
           {
            "cell_type": "code",
            "execution_count": null,
            "metadata": {},
            "outputs": [],
            "source": [
             "\\n",
             "fig, axs = plt.subplots(data[\\"<CND>\\"].nunique()*data[\\"PARTICIPANT_NUMBER\\"].nunique())\\n",
             "fig.set_figheight(60)\\n",
             "fig.set_figwidth(5)\\n",
             "\\n",
             "i = 0\\n",
             "for sliceidx in range(4):\\n",
             "    for partid in range(1,8):\\n",
             "        axs[i].set_xlim([min(data[\\"SOA_IN_MS\\"]),max(data[\\"SOA_IN_MS\\"])])\\n",
             "        axs[i].set_ylim([0,1])\\n",
             "        axs[i].title.set_text(\\"Participant: \\"+str(partid)+\\", Slice\\"+str(sliceidx))\\n",
             "        \\n",
             "        partdata = data[(data[\\"PARTICIPANT_NUMBER\\"] == partid) & (data[\\"<CND>\\"] == sliceidx)]\\n",
             "        \\n",
             "        axs[i].plot(partdata[\\"SOA_IN_MS\\"], partdata[\\"Relative\\"] , '-')\\n",
             "        i = i + 1\\n",
             "fig.tight_layout()"
            ]
           },
           {
            "cell_type": "code",
            "execution_count": null,
            "metadata": {},
            "outputs": [],
            "source": [
             "idx = data['PARTICIPANT_NUMBER']\\n",
             "nidx = data['PARTICIPANT_NUMBER'].nunique()\\n",
             "cnd = data[\\"<CND>\\"]\\n",
             "ncnd = data['<CND>'].nunique()\\n",
             "SOA = data['SOA_IN_MS'].values\\n",
             "nSOA = data['SOA_IN_MS'].nunique()\\n",
             "reps = data['reps'].values\\n",
             "probe_first_count = data['Relative'].values"
            ]
           },
           {
            "cell_type": "code",
            "execution_count": null,
            "metadata": {},
            "outputs": [],
            "source": [
             "def pf_tva_simple_toj_C_w(SOA, C, wp):\\n",
             "    vp = C*wp\\n",
             "    vr = C*(1-wp)\\n",
             "    left = (1-np.exp(-vp*abs(SOA))) + np.exp(-vp*abs(SOA)) * vp/(vp+vr)\\n",
             "    right = np.exp(-vr*abs(SOA))*vp/(vp+vr)\\n",
             "    return ((SOA<=0)*left  + (SOA>0)*right)"
            ]
           },
           {
            "cell_type": "code",
            "execution_count": null,
            "metadata": {},
            "outputs": [],
            "source": [
             "model = pymc3.Model()\\n",
             "with model: \\n",
             "   # priors from https://github.com/jt-lab/tvatoj-power#priors\\n",
             "    C_baseline_µ = pymc3.Normal('C_baseline_µ', mu=0.080, sd=0.050, shape=(cnd.nunique())) \\n",
             "    C_baseline_σ = pymc3.HalfCauchy('C_baseline_σ', beta=0.01, shape=(cnd.nunique()))\\n",
             "    C_baseline_e = pymc3.Normal('C_baseline_e', mu=0, sd=1, shape=(idx.nunique(), cnd.nunique()))\\n",
             "    C_baseline = pymc3.Deterministic('C', C_baseline_µ + C_baseline_σ * C_baseline_e)\\n",
             "\\n",
             "    #============================================\\n",
             "    wp_mu = pymc3.Normal('wp_mu', 0.5,0.2, shape=(cnd.nunique()))\\n",
             "    wp_sd = pymc3.HalfCauchy('wp_sd', 0.2, shape=(cnd.nunique()))\\n",
             "\\n",
             "    wp_e = pymc3.Normal('wp_e', mu=0, sd=1, shape=(idx.nunique(), cnd.nunique()))\\n",
             "    wp = pymc3.Deterministic('wp', wp_mu + wp_e * wp_sd)\\n",
             "    #============================================\\n",
             "\\n",
             "    theta = pymc3.Deterministic('theta', pf_tva_simple_toj_C_w(data['SOA_IN_MS'].values, \\n",
             "                                                               C_baseline[(idx, cnd)],\\n",
             "                                                               wp[(idx, cnd)])) \\n",
             "\\n",
             "\\n",
             "    y = pymc3.Binomial('y', n=cast(data['reps'], 'int64'), p=theta, \\n",
             "                       observed=cast(data['probe_first_count'], 'int64'), dtype='int64')\\n",
             "\\n",
             "    trace = pymc3.sample(5000, tune=2000, \\n",
             "                         init='adapt_diag', target_accept=0.95, cores=4, return_inferencedata=False)"
            ]
           },
           {
            "cell_type": "code",
            "execution_count": null,
            "metadata": {},
            "outputs": [],
            "source": [
             "fig, axes = plt.subplots(7*4,1, figsize=(10,2.5*4), sharex=True, sharey=True)\\n",
             "\\n",
             "fig.set_figheight(60)\\n",
             "fig.set_figwidth(5)\\n",
             "\\n",
             "condition_names = [\\"Slice 1\\", \\"Slice 2\\", \\"Slice 3\\", \\"Slice 4\\"]\\n",
             "\\n",
             "participants = list(map(lambda x: \\"Participant \\"+str(x), data['PARTICIPANT_NUMBER'].values))\\n",
             "\\n",
             "idx = 0\\n",
             "\\n",
             "for i in range(0,7):\\n",
             "    for j in range(4):\\n",
             "        pymc3.plot_posterior(np.array(trace['C'][:,i,j], dtype=float), ax=axes[idx], kind = 'hist',figsize=(10,3))\\n",
             "        axes[idx].set_title(\\"participants mean visual processing capacity of Condition ({},{})\\".format(condition_names[j],participants[i]),fontsize=16)\\n",
             "        axes[idx].set_xlabel(r\\"C (Hz)\\",fontsize=16)\\n",
             "        idx = idx + 1\\n",
             "plt.tight_layout()\\n",
             "plt.savefig('./results/C-mean.png')"
            ]
           }
          ],
          "metadata": {
           "kernelspec": {
            "display_name": "Python + PYMC 4",
            "language": "python",
            "name": "pymc4_env_3_10"
           },
           "language_info": {
            "codemirror_mode": {
             "name": "ipython",
             "version": 3
            },
            "file_extension": ".py",
            "mimetype": "text/x-python",
            "name": "python",
            "nbconvert_exporter": "python",
            "pygments_lexer": "ipython3",
            "version": "3.9.11"
           },
           "orig_nbformat": 4
          },
          "nbformat": 4,
          "nbformat_minor": 2
         }             
        `)
        exec(`chmod 777 -R ${folder}/jupyter/experiments`, (error, stdout, stderr) => {
          if (error) {
              console.log(`error: ${error.message}`);
              return;
          }
          if (stderr) {
              console.log(`stderr: ${stderr}`);
              return;
          }
          //console.log(`stdout: ${stdout}`);
      });
        //file written successfully
      } catch (err) {
        console.error(err)
      }
      
      res.send("success")
      return   
})

export {jupyterRouter}