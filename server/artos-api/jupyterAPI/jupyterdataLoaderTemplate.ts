export class jupyterdataLoaderTemplate{
    static dataloader = `data = None
with open('data.json') as f:
    datatxt = f.read()
    datatxt = datatxt.replace('\"{', '{')
    datatxt = datatxt.replace('}\"', '}')
    datatxt = datatxt.replace(\"'\", '\"')
    datatxt = datatxt.replace('True', 'true')
    datatxt = datatxt.replace('False', 'false')
    with open('data/HLdata2.json', 'w') as f2:
        f2.write(datatxt)
    data = json.loads(datatxt)
df = pd.json_normalize(data)

# display(df)

df['PARTICIPANT_NUMBER'] = df['participant_id']
df['SOA_IN_MS'] = df['result.soa'].astype('int') / 60
df['PROBE_FIRST_RESPONSE'] = pd.Series(
    list(map(lambda x: 1 if x == 'true' else 0, df['result.probeFirstSelected']))
)
df['CONDITION'] = df['result.condition']

df =df[~(df['result.valid'] == 'false')]
#display(df)

tvatojdata = df[
    ['PARTICIPANT_NUMBER', 'PROBE_FIRST_RESPONSE', 'SOA_IN_MS', 'CONDITION']
]



tvatojdata = tvatojdata.groupby(
    ['SOA_IN_MS', 'PARTICIPANT_NUMBER', 'CONDITION'], as_index=False
).agg({'PROBE_FIRST_RESPONSE': ['sum', 'size']})
tvatojdata['Relative'] = (
    tvatojdata['PROBE_FIRST_RESPONSE']['sum']
    / tvatojdata['PROBE_FIRST_RESPONSE']['size']
)

tvatojdata['Relative'] = 1 - tvatojdata['Relative']
tvatojdata['reps'] = tvatojdata['PROBE_FIRST_RESPONSE']['size']
tvatojdata['probe_first_count'] = tvatojdata['PROBE_FIRST_RESPONSE']['size']-tvatojdata['PROBE_FIRST_RESPONSE']['sum']
del tvatojdata['PROBE_FIRST_RESPONSE']
#tvatojdata = tvatojdata[tvatojdata['PARTICIPANT_NUMBER'] =='5']
print(sum(tvatojdata['reps']))
tvatojdata = tvatojdata[(tvatojdata['SOA_IN_MS']<0.083333)]
display(tvatojdata)`
}