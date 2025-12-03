USE ClusterDb;


PRINT 'Executing CreateDummyCandidate to populate dummy candidates...';

-- Check if the stored procedure exists before calling it (good practice for idempotency)
IF OBJECT_ID(N'[dbo].[CreateDummyCandidate]', N'P') IS NOT NULL
BEGIN
	exec CreateDummyCandidate '1CFG1109X','Barry','Stone','18/01/1979 00:00','A7022BA','B008622','TRUE','Male','British','White','DELIUS','ACI','C06';
	exec CreateDummyCandidate '1CFG3305M','Clark','Queen','16/07/1985 00:00','A7373XA','B004786','TRUE','Female','British','White','DELIUS','HII','N28';
	exec CreateDummyCandidate '1CFG3647Z','Hal','Parker','31/08/2000 00:00','A1118OA','B006754','TRUE','Male','British','Black','DELIUS','KMI','LCS';
	exec CreateDummyCandidate '1CFG4819M','Arthur','Stone','08/03/1961 00:00','A6972XA','B008866','TRUE','Female','British','White','NOMIS','LPI','MRS';
	exec CreateDummyCandidate '1CFG3977A','Oliver','Kent','16/08/1966 00:00','A3749HA','B004529','FALSE','Male','British','White','NOMIS','MRI','MCG';
	exec CreateDummyCandidate '1CFG4580T','Barry','Jordan','18/06/1979 00:00','A6102NA','B007498','FALSE','Male','British','Black','NOMIS','RSI','N50';
	exec CreateDummyCandidate '1CFG7754K','Victor','Kent','21/10/1956 00:00','A2817BA','B003913','FALSE','Male','British','White','NOMIS','PNI','CMB';
	exec CreateDummyCandidate '1CFG1873Q','Arthur','Wayne','15/08/1988 00:00','A8654XA','B004044','TRUE','Male','British','White','NOMIS','BAI','C17';
	exec CreateDummyCandidate '1CFG5221C','Victor','Parker','14/09/1977 00:00','A3576SA','B003506','TRUE','Male','British','White','DELIUS','BXI','LDN';
	exec CreateDummyCandidate '1CFG5006P','Clark','Kent','09/09/1988 00:00','A8065EA','B001407','TRUE','Female','British','White','DELIUS','FMI','N07';
	exec CreateDummyCandidate '1CFG2001T','Oliver','Jordan','30/09/2004 00:00','A6587YA','B003237','TRUE','Female','British','Black','NOMIS','ISI','N21';
	exec CreateDummyCandidate '1CFG5437L','Hal','Parker','25/09/1956 00:00','A6952ZA','B003171','TRUE','Male','British','White','DELIUS','HLI','N55';
	exec CreateDummyCandidate '1CFG2883L','Oliver','Prince','29/08/1961 00:00','A3402IA','B007887','TRUE','Male','British','White','DELIUS','HMI','HBS';
	exec CreateDummyCandidate '1CFG2167R','Bruce','Allen','27/08/1969 00:00','A7364GA','B009234','TRUE','Female','British','White','NOMIS','LEI','YSW';
	exec CreateDummyCandidate '1CFG9064R','Bruce','Jordan','19/07/1982 00:00','A6222SA','B004752','TRUE','Male','British','White','NOMIS','NHI','C05';
	exec CreateDummyCandidate '1CFG1900I','Oliver','Curry','30/06/1983 00:00','A9293CA','B005270','TRUE','Male','British','White','NOMIS','LHI','C09';
	exec CreateDummyCandidate '1CFG2847D','Bruce','Jordan','17/09/1991 00:00','A5884JA','B005459','TRUE','Female','British','Black','DELIUS','BMI','N52';
	exec CreateDummyCandidate '1CFG7892B','Arthur','Wayne','22/08/1952 00:00','A9162QA','B006933','TRUE','Male','British','White','DELIUS','DGI','N30';
	exec CreateDummyCandidate '1CFG6906H','Diana','Curry','09/08/1980 00:00','A6150IA','B005260','TRUE','Male','British','White','NOMIS','DHI','SWM';
	exec CreateDummyCandidate '1CFG1479H','Peter','Prince','18/07/1956 00:00','A6948JA','B004442','TRUE','Female','British','White','NOMIS','HEI','WMP';
	exec CreateDummyCandidate '1CFG7788V','Diana','Lance','27/10/1953 00:00','A9649AB','B004219','TRUE','Male','British','White','NOMIS','SFI','C11';
	exec CreateDummyCandidate '1CFG1553F','Victor','Jordan','08/12/1978 00:00','A9860RA','B008576','TRUE','Female','British','White','DELIUS','FHI','DBS';
	exec CreateDummyCandidate '1CFG4867Q','Oliver','Jordan','03/02/2000 00:00','A1567LA','B008340','TRUE','Male','British','Black','NOMIS','FWI','N31';
	exec CreateDummyCandidate '1CFG5469L','Peter','Queen','18/11/1953 00:00','A4692DA','B001257','TRUE','Male','British','White','NOMIS','LII','LNS';
	exec CreateDummyCandidate '1CFG5502H','Bruce','Kent','11/02/1978 00:00','A1411YA','B009194','TRUE','Male','British','Asian','DELIUS','WLI','NSP';
	exec CreateDummyCandidate '1CFG2825E','Peter','Prince','23/12/1975 00:00','A7907DA','B009268','TRUE','Female','British','White','DELIUS','PBI','CBS';
	exec CreateDummyCandidate '1CFG8516T','Dinah','Kent','19/06/1954 00:00','A7400NA','B009761','TRUE','Female','British','White','NOMIS','NWI','N56';
	exec CreateDummyCandidate '1CFG5298O','Victor','Jordan','28/07/1961 00:00','A8430TA','B007256','TRUE','Male','British','White','NOMIS','MTI','HFS';
	exec CreateDummyCandidate '1CFG9775P','Barry','Stone','14/08/1960 00:00','A1067SA','B008155','TRUE','Female','British','White','DELIUS','LNI','N02';
	exec CreateDummyCandidate '1CFG6849L','Bruce','Parker','20/02/1977 00:00','A7483AB','B009597','TRUE','Male','British','White','DELIUS','NLI','NBR';
	exec CreateDummyCandidate '1CFG8347B','Clark','Queen','06/10/2005 00:00','A1056JA','B005305','TRUE','Female','British','White','NOMIS','KVI','N23';
	exec CreateDummyCandidate '1CFG4189E','Bruce','Curry','14/04/1956 00:00','A3064CA','B006480','TRUE','Female','British','Black','NOMIS','HHI','N32';
	exec CreateDummyCandidate '1CFG5514M','Clark','Curry','28/05/1962 00:00','A7236EA','B005528','TRUE','Female','British','White','NOMIS','DMI','N54';
	exec CreateDummyCandidate '1CFG5252T','Peter','Queen','23/09/1964 00:00','A4570WA','B008719','TRUE','Female','British','White','DELIUS','SDI','SSP';
	exec CreateDummyCandidate '1CFG6789D','Clark','Parker','24/10/1968 00:00','A6440GA','B002584','TRUE','Male','British','White','DELIUS','SLI','N35';
	exec CreateDummyCandidate '1CFG1848U','Barry','Jordan','09/06/1981 00:00','A7126QA','B004852','TRUE','Female','British','White','DELIUS','SPI','N37';
	exec CreateDummyCandidate '1CFG4892S','Arthur','Allen','09/10/1960 00:00','A6249NA','B008270','TRUE','Male','British','White','NOMIS','WHI','N59';
	exec CreateDummyCandidate '1CFG5866O','Peter','Jordan','07/10/2001 00:00','A9186BA','B008561','TRUE','Female','British','Black','NOMIS','CWI','C19';
	exec CreateDummyCandidate '1CFG7842D','Diana','Queen','15/07/1982 00:00','A2699ZA','B005033','TRUE','Female','British','White','NOMIS','DAI','DCP';
	exec CreateDummyCandidate '1CFG8262O','Peter','Curry','02/07/1984 00:00','A2865UA','B001022','TRUE','Male','British','White','NOMIS','BLI','C15';
	exec CreateDummyCandidate '1CFG6353O','Hal','Stone','15/01/1952 00:00','A4973LA','B002433','TRUE','Male','British','White','NOMIS','GMI','DRS';
	exec CreateDummyCandidate '1CFG2308N','Clark','Parker','26/06/2004 00:00','A3961UA','B006501','TRUE','Male','British','White','NOMIS','EWI','ASP';
END
ELSE
BEGIN
    PRINT 'Stored procedure [dbo].[CreateDummyCandidate] not found. Skipping call.';
END