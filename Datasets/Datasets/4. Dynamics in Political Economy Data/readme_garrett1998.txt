Explanation of garrett1998.dta

Use Geoff Garrett's data (from Partisan Politics in the Global Era),
garrett1998.dta. . It should be pretty clear what the vars are, or you can
read his section 5.3. (The vars are briefly described in the stata
file, type desc in stata.) Note that it is already in xt and ts format.

(The Icc are country dummies, the perxxyy are dummies for the period
19xx-yy (one year is omitted so no multicolinearity, the key variables
are leftlab (the political power of left parties in the govt), corp (a
measure of corporatism or how encompassing the labor movement is and
cl_int which is the product of the two - the first two vars are
indices which run form 0 to 5 and 0 to 4 respectively. The dep vars
are either unem(ployment), infl(ation) or gdp(growth) with lags being
indicated by a suffixed "l". oild and oecd_dem are controls for oil
dependency (and prices) and the growth rate in all oecd
countries. Data are for 14 oecd countries from 1966-90. The data are
already in xt form. Use stata xt commands). 

For replication, the dep var in Table 2 is gdp.


The country codes are as follows

2 US
20 Canada
200 UK
210 Netherlands
211 Belgium
220 France
260 Germany
305 Austria
325 Italy
375 Finland
380 Sweden
385 Norway 
390 Denmark
740 Japan
