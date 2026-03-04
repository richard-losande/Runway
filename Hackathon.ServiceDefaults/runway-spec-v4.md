# RUNWAY

**Product & Engineering Specification**

```

```

| **Product**                       | **Platform**                      |
|                                   |                                   |
| **Runway --- Financial Resilience | Sprout Payroll Web App            |
| Calculator**                      | (sprout.ph)                       |

| **Version**                       | **Framework**                     |
|                                   |                                   |
| v2.0 --- Post-Hackathon Build     | Superpowers-compatible            |
| Spec                              |                                   |

| **Owner**                         | **Status**                        |
|                                   |                                   |
| Sprout Product & Engineering      | Ready for implementation planning |

| **One-sentence product definition**                                   |
|                                                                       |
| *Runway tells Sprout payroll users exactly how many days their        |
| savings would last if their income stopped today --- and routes them  |
| to the right Sprout Finances product to extend that number.*          |

### 1. Product Context

### 1.1 What Runway Is

Runway is a financial resilience calculator embedded inside the Sprout
payroll employee experience. It answers one question: how many days
could you last if your income stopped today?

The core formula is:

| **Runway Days = Liquid Cash ÷ Daily Burn Rate**                       |
|                                                                       |
| where Daily Burn Rate = Monthly Burn ÷ 30                             |

For the demo persona Alex Garcia:

-   Liquid Cash: ₱180,000 (user-entered, all bank accounts + e-wallets)

-   Monthly Burn: ₱52,400 (computed from 4 months of GCash transaction
    data)

-   Daily Burn Rate: ₱52,400 ÷ 30 = ₱1,746.67/day

-   Runway: ₱180,000 ÷ ₱1,746.67 = 103 days

### 1.2 Why It Lives Inside Sprout

Sprout already has the two hardest inputs: salary data (from payroll)
and spending data (from GCash if connected, or via CSV upload). No other
financial wellness product in the Philippines has both without the user
doing manual data entry. Runway uses this data advantage to give a
number no competitor can give without significant friction.

The secondary advantage is trust context. A financial wellness insight
delivered on payday, inside the tool Alex's employer already uses,
carries a different weight than a standalone fintech product.

### 1.3 Primary User

  ---------------- ------------------------------------------------------
  **Attribute**    **Detail**

  Name             Alex Garcia

  Age / Stage      28, mid-career professional

  Employer context Uses Sprout-powered payroll

  Financial        Stable income, growing lifestyle spend --- "Lifestyle
  pattern          Inflator" archetype

  Monthly gross    ₱35,000

  Net take-home    ₱28,500 (after SSS, PhilHealth, Pag-IBIG, withholding,
                   salary loan)

  Current savings  ₱180,000 (liquid --- bank + GCash combined)

  Monthly burn     ₱52,400 (exceeds take-home by ₱23,900 --- drawing down
                   savings)

  Baseline runway  103 days --- Stable Zone
  ---------------- ------------------------------------------------------

### 1.4 Entry Point

Runway surfaces as a contextual card attached to the payslip in the My
Pay feed --- not a standalone product tab. When Alex's September 30
payslip processes, a Runway card appears inline with the payslip.

| **Entry trigger: payslip processed event**                            |
|                                                                       |
| -   Card headline: "What's your financial runway?"                 |
|                                                                       |
| -   CTA: "Check My Runway →" (spr-button tone="success")          |
|                                                                       |
| -   The card only shows once per payday cycle unless dismissed        |
|                                                                       |
| -   If Alex has already completed Runway setup, the card shows her    |
|     current score instead                                             |

### 2. Data Strategy

### 2.1 Data Ownership --- What Sprout Has vs. What the User Brings

This distinction is foundational to Runway's design and must be
reflected in all copy, consent flows, and engineering decisions.

  ------------------ --------------- ------------------------- ------------
  **Data Type**      **Who Has It**  **How Runway Gets It**    **Screen**

  Gross salary,      Sprout --- from Pre-filled automatically. Screen 2
  deductions, net    payroll record  User sees it, confirms    
  pay, loan balance                  it.                       

  Transaction /      The user ---    User chooses to share it  Screen 3
  spending history   locked in their via GCash API or CSV      
                     GCash, bank     upload. Runway never has  
                     app, or credit  access without explicit   
                     card statement  user action.              

  Liquid savings     The user ---    User types it manually on Screen 2
  amount             Sprout has no   Screen 2.                 
                     visibility                                
  ------------------ --------------- ------------------------- ------------

| **Copy principle: the user opens the door, not Sprout**               |
|                                                                       |
| *Sprout knows your salary because your employer gave that to us.      |
| Sprout does not know your spending --- you do. Runway's job is to    |
| make it easy for you to share your own data with yourself in a useful |
| way.*                                                                 |
|                                                                       |
| This framing must be consistent across all screens. Never say "we    |
| pulled your transactions" or "we already have this." Say "you     |
| connected your GCash" or "your file was processed."                |

### 2.2 Three-Tier Input Model

Runway presents three paths for spending data. All three are shown
simultaneously on Screen 3. The user chooses. The demo preloads Alex's
dataset regardless of which path is chosen --- in production, each path
produces real data.

  ---------- --------------------------- ---------------- -------------------------
  **Tier**   **Source**                  **Accuracy**     **User Action**

  1 ---      User's GCash transaction   High             Tap "Connect GCash" →
  GCash API  history. Read-only. User                     OAuth flow → done
             authorises access.                           

  2 --- CSV  User exports CSV from their High             Export from bank → upload
  Upload     bank app (GCash, BDO, BPI,                   file → Runway parses it
             UnionBank, Maya, RCBC) and                   
             uploads the file.                            

  3 ---      User types a single monthly Low ---          Type one number
  Estimate   spend estimate.             self-reported,   
                                         no breakdown     
  ---------- --------------------------- ---------------- -------------------------

| **Progressive degradation rule**                                      |
|                                                                       |
| If the user declines GCash consent: offer CSV. If they skip CSV:      |
| offer Estimate. Never block the flow. A low-accuracy runway number is |
| more useful than no number at all.                                    |

### 2.3 Payroll Data (Pre-filled by Sprout)

The following fields are sourced from the Sprout payroll record and
displayed on Screen 2. The user sees them but does not enter them. The
spr-badge "Pre-filled by Sprout" makes the source visible.

-   Monthly Gross Salary

-   SSS / PhilHealth / Pag-IBIG contributions

-   Withholding Tax

-   Salary Loan deduction (if active)

-   Net Take-Home Pay (computed: gross minus all deductions)

Screen 2 headline: "Your payroll, ready to go"

Sub-line: "We pre-filled this from your September 30 payslip. Just
confirm your savings below."

### 2.4 Progressive Consent

Two consent moments in the flow. Each is specific about what data moves,
where it goes, and what does not happen.

  ---------- --------------- ------------------------------ ----------------
  **Step**   **Data**        **Copy**                       **If Declined**

  1 ---      Payroll read    "Runway uses the payroll      Flow cannot
  Screen 2   (within Sprout) figures above to calculate     continue ---
                             your burn gap. This stays      payroll is the
                             within Sprout and is never     minimum required
                             shared with your employer."   input

  2 ---      Transaction     See tier-specific consent copy Degrades to
  Screen 3   data (user's   in Section 2B.1                Estimate tier.
             own)                                           Flow continues.
  ---------- --------------- ------------------------------ ----------------

### 2.5 Category System

Spending is grouped into 9 categories plus Miscellaneous. Categories
carry an internal tier (Essential / Discretionary / Committed) that
drives scenario generation rules. Tiers are not shown as labels in the
main UI --- they operate silently.

  ------------------ --------------- ------------- ---------------------------
  **Category**       **Tier**        **Demo Avg /  **Notes**
                                     Month**       

  Food & Dining      Discretionary   ₱18,200       Restaurants, delivery,
                                                   coffee

  Groceries & Market Essential       ₱9,400        Supermarkets, wet market,
                                                   convenience

  Bills & Utilities  Essential       ₱8,100        Electricity, water,
                                                   internet, phone

  Transport          Essential       ₱7,600        Ride-hailing, fuel, transit

  Shopping           Discretionary   ₱5,800        Retail, online
                                                   marketplaces, clothing

  Health & Wellness  Essential       ₱3,200        Pharmacy, clinic, gym, HMO
                                                   premium

  Housing            Essential       ₱0*          *Alex rents --- demo
                                                   dataset has no housing row
                                                   because rent is a direct
                                                   bank transfer not captured
                                                   in GCash export. See 2B.5.

  Transfers &        Committed       ₱5,000        GCash padala, bank transfer
  Remittances                                      to family

  Entertainment &    Discretionary   ₱3,300        Streaming, gaming, events
  Subscriptions                                    

  Miscellaneous      Discretionary   ₱1,400        ATM withdrawals,
                                                   uncategorised
  ------------------ --------------- ------------- ---------------------------

| **Remittances treatment**                                             |
|                                                                       |
| -   Included in monthly burn --- it is a real monthly cash outflow    |
|                                                                       |
| -   Shown as its own line in the burn breakdown with label            |
|     "Transfers & Family"                                            |
|                                                                       |
| -   No cut scenario ever generated from this category                 |
|                                                                       |
| -   If remittances > 15% of total burn: diagnosis copy acknowledges  |
|     it explicitly --- "₱X of your monthly outflow goes to family --- |
|     that's a commitment, not a cut"                                 |
|                                                                       |
| -   Custom scenario is the only way a user can model reducing         |
|     remittances --- they initiate it, Runway never suggests it        |

Full categorisation pipeline spec: see Section 2B.

## 2B. Transaction Processing Pipeline

This section specifies how Runway processes a raw CSV or structured data
file from a user's bank or e-wallet into a categorised monthly burn
rate. All processing happens server-side. Raw transaction data is
transmitted to Sprout's backend over TLS and is not stored beyond the
current session.

**2B.1 Consent Copy (Screen 3)**

When the user selects Tier 1 (GCash API) or Tier 2 (CSV upload), a
consent notice is shown before data is sent. The copy is light but
specific --- it names what is transmitted, where it goes, and what
happens to it.

| **Consent notice --- Tier 1 (GCash API):**                            |
|                                                                       |
| *"Runway will read your last 4 months of GCash transactions to       |
| calculate your spending average. This is processed on Sprout's       |
| servers and is not stored after your session ends."*                 |
|                                                                       |
| **Consent notice --- Tier 2 (CSV upload):**                           |
|                                                                       |
| *"Your file is processed on Sprout's servers to calculate your      |
| spending average. It is not stored after your session ends. We never  |
| share it with your employer."*                                       |
|                                                                       |
| **Consent copy principles:**                                          |
|                                                                       |
| -   Name the action ("read", "processed") --- not "access" or   |
|     "collect"                                                       |
|                                                                       |
| -   Specify the scope ("last 4 months") --- not open-ended          |
|                                                                       |
| -   State what does NOT happen ("not stored", "not shared with     |
|     employer")                                                       |
|                                                                       |
| -   No checkbox required --- proceeding past this notice constitutes  |
|     consent                                                           |
|                                                                       |
| -   Link to privacy policy, but do not require reading it             |

**2B.2 Pipeline Overview**

The categorisation pipeline runs in five sequential stages. The LLM
layer (Stage 4) only receives transactions that the rule-based lookup
(Stage 3) could not resolve with high confidence.

```
Raw CSV / GCash API response │ ▼ Stage 1 --- Format Detection Identify
  source bank/app. Map columns to canonical schema. │ ▼ Stage 2 ---
  Transaction Normalisation Parse amounts, dates. Exclude non-spend rows
  (transfers in, refunds, reversals). │ ▼ Stage 3 --- Rule-Based Lookup
  Match normalised description against merchant seed list. Output: {
  category, merchant, confidence: "high" } or unresolved │ ▼
  (unresolved only) Stage 4 --- OpenAI Fallback Batch unresolved
  descriptions → GPT-4o mini → category + confidence score Output: {
  category, merchant, confidence: 0.0--1.0 } │ ▼ Stage 5 --- Aggregation
  Group by category × month. Compute averages. Flag low-confidence rows.
  Output: CategoryBreakdown + CorrectionCandidates[]
```

**2B.3 Stage 1 --- Format Detection**

Each supported bank/app exports a different CSV structure. Format is
detected by inspecting the header row and column patterns.

  ------------ ------------------- ----------------- ----------------- --------------
  **Source**   **Detection         **Amount Column** **Description     **Date
               Signal**                              Column**          Format**

  GCash        Header contains     "Credit" /      "Remarks"       MM/DD/YYYY
               "Reference No." + "Debit"                           
               "Remarks"         separate cols                       

  BDO          Header contains     "Amount"        "Transaction     MM/DD/YYYY
               "Transaction                         Details"         HH:MM
               Details" +                                             
               "Running Balance"                                     

  BPI          Header contains     "Credit Amount" "Description"   MM-DD-YYYY
               "Reference         / "Debit                           
               Number" +          Amount"                            
               "Description"                                         

  UnionBank    Header contains     "Withdrawals" / "Particulars"   MM/DD/YY
               "Particulars" +   "Deposits"                        
               "Withdrawals"                                         

  Maya         Header contains     "Amount"        "Notes" +       YYYY-MM-DD
               "Type" +                            "Type"          HH:MM:SS
               "Notes"                                               

  RCBC         Header contains     "Debit" /       "Remarks"       DD/MM/YYYY
               "Debit" +         "Credit"                          
               "Credit" +                                            
               "Balance"                                             

  Unknown      No match above      Heuristic:        Heuristic:        Auto-detect
                                   largest numeric   longest text col  
                                   col                                 
  ------------ ------------------- ----------------- ----------------- --------------

| **Unknown format handling:**                                          |
|                                                                       |
| -   Attempt heuristic detection (largest numeric column = amount,     |
|     longest text column = description)                                |
|                                                                       |
| -   If amount column cannot be determined, show error: "We couldn't |
|     read this file format. Try exporting from GCash, BDO, BPI,        |
|     UnionBank, Maya, or RCBC."                                       |
|                                                                       |
| -   Never silently fail --- always give the user a path forward       |

**2B.4 Stage 2 --- Transaction Normalisation**

Raw rows are normalised into a canonical transaction schema and filtered
to spending transactions only.

```
type Transaction = { date: Date amount: number // always positive
  (outflow) rawDesc: string // original description string normDesc:
  string // cleaned description (uppercase, stripped) source: BankSource
  // which bank format was detected category?: CategoryKey // filled by
  Stage 3 or 4 merchant?: string // normalised merchant name confidence?:
  "high" \| number // "high" from rule lookup, 0-1 from LLM }
```

Exclusion rules --- these row types are excluded from burn calculation:

  ------------------ ------------------------ ----------------------------
  **Row Type**       **Detection Pattern**    **Treatment**

  Incoming transfer  Amount is a credit       Excluded entirely
  / credit           (positive inflow)        

  Refund / reversal  Description contains     Excluded from burn
                     "REFUND",              
                     "REVERSAL",            
                     "RETURN", "REF"      

  Inter-account      Amount matches a         Excluded --- flag for review
  transfer (own)     same-day credit from     if uncertain
                     same account             

  Loan / credit      Description contains     Included in Bills &
  payment            "LOAN PAYMENT",        Utilities
                     "CREDIT CARD", "BILL  
                     PAYMENT"                

  ATM withdrawal     Description contains     Included as Miscellaneous
                     "ATM", "WITHDRAWAL"  (cannot classify further)

  Salary credit      Large recurring credit   Excluded --- this is income
                     matching payroll amount  not spend
                     ±10%                     
  ------------------ ------------------------ ----------------------------

**2B.5 Stage 3 --- Rule-Based Merchant Lookup**

The seed list maps normalised description patterns to merchant names and
categories. Pattern matching is case-insensitive substring match on
normDesc. Longer patterns are matched first (specificity wins).

Normalisation applied to rawDesc before matching:

-   Convert to uppercase

-   Remove special characters except spaces and asterisks

-   Collapse multiple spaces to single space

-   Strip leading/trailing whitespace

-   Remove common bank-added suffixes: transaction reference numbers,
    timestamps, branch codes

**Seed List --- Food & Dining (Discretionary)**

  ---------------------- ------------------ -----------------------------
  **Pattern**            **Merchant Name**  **Notes**

  GRAB*FOOD             Grab Food          GCash and credit card format

  GRABFOOD               Grab Food          Alternative format

  GRAB FOOD              Grab Food          Spaced format

  FOODPANDA              foodpanda          

  MCDONALDS              McDonald's        

  MC DONALDS             McDonald's        Spaced variant

  JOLLIBEE               Jollibee           

  KFC                    KFC                Substring --- check not part
                                            of longer string

  CHOWKING               Chowking           

  MANG INASAL            Mang Inasal        

  GREENWICH              Greenwich          

  RED RIBBON             Red Ribbon         

  STARBUCKS              Starbucks          

  BOS COFFEE             Bo's Coffee       

  SHAKEYS                Shakey's          

  PIZZA HUT              Pizza Hut          

  DOMINOS                Domino's          

  ARMY NAVY              Army Navy          

  ZARKS                  Zark's Burgers    

  BONCHON                BonChon            

  MAX RESTAURANT         Max's Restaurant  

  YELLOW CAB             Yellow Cab         

  SWEET CORN             Sweet Corn         

  TROPICAL HUT           Tropical Hut       

  GOLDILOCKS             Goldilocks         

  ANGEL'S PIZZA         Angel's Pizza     

  SBARRO                 Sbarro             

  TOKYO TOKYO            Tokyo Tokyo        

  PANCAKE HOUSE          Pancake House      

  COUNTRY STYLE          Country Style      
  ---------------------- ------------------ -----------------------------

**Seed List --- Groceries & Market (Essential)**

  ---------------------- ------------------ -----------------------------
  **Pattern**            **Merchant Name**  **Notes**

  SM SUPERMARKET         SM Supermarket     

  SM HYPERMARKET         SM Hypermarket     

  ROBINSONS SUPERMARKET  Robinsons          Match before shorter
                         Supermarket        ROBINSONS

  PUREGOLD               Puregold           

  S&R                    S&R Membership     Short pattern --- risk of
                         Shopping           false match

  SNR                    S&R Membership     Alternative export format
                         Shopping           

  WALTERMART             WalterMart         

  METRO GAISANO          Metro Gaisano      

  SHOPWISE               Shopwise           

  RUSTANS SUPERMARKET    Rustan's          
                         Supermarket        

  LANDERS                Landers Superstore 

  ALLDAY SUPERMARKET     AllDay Supermarket 

  7-ELEVEN               7-Eleven           

  7 ELEVEN               7-Eleven           

  7ELEVEN                7-Eleven           

  MINISTOP               Ministop           

  FAMILYMART             FamilyMart         

  FAMILY MART            FamilyMart         

  ALFAMART               Alfamart           

  THE LANDMARK FOOD      The Landmark       Match before plain LANDMARK
                         Supermarket        

  LANDMARK SUPERMARKET   The Landmark       
                         Supermarket        

  MERKADO                Merkado            Online palengke

  LINIS                  Linis.ph           Grocery delivery

  GRAB MART              GrabMart           Grocery via Grab

  GRABMART               GrabMart           

  PANDAMART              PandaMart          Grocery via foodpanda

  LAZMART                LazMart            Lazada grocery

  HONEST BEE             honestbee          
  ---------------------- ------------------ -----------------------------

**Seed List --- Bills & Utilities (Essential)**

  ---------------------- ------------------ -----------------------------
  **Pattern**            **Merchant Name**  **Notes**

  MERALCO                Meralco            

  MECO                   Meralco            Alternative code in some
                                            exports

  MANILA WATER           Manila Water       

  MAYNILAD               Maynilad           

  GLOBE TELECOM          Globe              

  GLOBE*                Globe              Wildcard for Globe sub-brands

  SMART COMMUNICATIONS   Smart              

  SMART*                Smart              

  DITO TELECOMMUNITY     Dito               

  PLDT                   PLDT               

  CONVERGE               Converge ICT       

  SKYCABLE               Sky Cable          

  SKY CABLE              Sky Cable          

  CIGNAL                 Cignal             

  NETFLIX                Netflix            May appear as NETFLIX.COM

  SPOTIFY                Spotify            May appear as SPOTIFY AB

  YOUTUBE PREMIUM        YouTube Premium    

  GOOGLE*               Google             Google Play, YouTube ---
                                            check sub-pattern

  APPLE.COM/BILL         Apple              iCloud, Apple TV+
                         Subscriptions      

  BAYAD CENTER           Bayad Center       Utility aggregator

  BAYAD*                Bayad Center       

  ECPAY                  ECPay              Bill payment aggregator

  PAYNAMICS              Paynamics          Payment gateway for bills

  TRUEMONEY              TrueMoney          Bills payment
  ---------------------- ------------------ -----------------------------

**Seed List --- Transport (Essential)**

  ---------------------- ------------------ -----------------------------
  **Pattern**            **Merchant Name**  **Notes**

  GRAB*TRANS            Grab Car           Credit card export format

  GRAB CAR               Grab Car           

  GRABCAR                Grab Car           

  GRAB*EXPRESS          Grab Express       Parcel delivery --- classify
                                            as Transport

  ANGKAS                 Angkas             

  MOVE IT                Move It            

  MOVEIT                 Move It            

  AF PAYMENTS            Beep Card          Transit card top-up
                         (MRT/LRT)          

  BEEP                   Beep Card          

  PETRON                 Petron             Fuel

  SHELL                  Shell              Fuel --- check not Shell gift
                                            card

  CALTEX                 Caltex             Fuel

  PHOENIX PETROLEUM      Phoenix Fuels      

  SEAOIL                 Seaoil             Fuel

  GRAB BIKE              Grab Bike          

  JOYRIDE                JoyRide            Motorcycle ride-hailing

  OWTO                   Owto               Ride-hailing

  HYPE                   Hype               Ride-hailing

  LTFRB                  LTFRB              Transport regulatory ---
                                            classify as Transport

  TOLL                   Toll Fee           SCTEX, NLEX, CAVITEX etc.

  EASYTRIP               EasyTrip           Toll RFID top-up

  AUTOSWEEP              AutoSweep          Toll RFID top-up
  ---------------------- ------------------ -----------------------------

**Seed List --- Shopping (Discretionary)**

  ---------------------- ------------------ -----------------------------
  **Pattern**            **Merchant Name**  **Notes**

  LAZADA                 Lazada             

  SHOPEE                 Shopee             

  ZALORA                 Zalora             

  SHEIN                  Shein              

  TEMU                   Temu               

  SM STORE               SM Store (Dept)    Separate from SM Supermarket

  SM DEPARTMENT          SM Store (Dept)    

  ROBINSONS DEPT         Robinsons Dept     Match after ROBINSONS
                         Store              SUPERMARKET

  THE LANDMARK           The Landmark       Match after LANDMARK
                                            SUPERMARKET

  H&M                    H&M                

  ZARA                   Zara               

  UNIQLO                 Uniqlo             

  NATIONAL BOOK STORE    National Book      
                         Store              

  NBS                    National Book      Short --- risk of false match
                         Store              

  FULLY BOOKED           Fully Booked       

  TRUE VALUE             True Value         

  ACE HARDWARE           Ace Hardware       

  HANDYMAN               Handyman           

  ISTORE                 iStore             Apple reseller

  BEYOND THE BOX         Beyond the Box     Apple reseller

  SILICON VALLEY         Silicon Valley     Electronics

  ABENSON                Abenson            Appliances

  CD-R KING              CD-R King          

  MEMORIZE               Memo Xpress        

  MEMO EXPRESS           Memo Xpress        
  ---------------------- ------------------ -----------------------------

**Seed List --- Health & Wellness (Essential)**

  ---------------------- ------------------ -----------------------------
  **Pattern**            **Merchant Name**  **Notes**

  MERCURY DRUG           Mercury Drug       

  MERCURYDRUG            Mercury Drug       

  WATSONS                Watsons            

  ROSE PHARMACY          Rose Pharmacy      

  GENERIKA               Generika           

  SOUTHSTAR DRUG         Southstar Drug     

  THE GENERICS PHARMACY  The Generics       
                         Pharmacy           

  TGP                    The Generics       
                         Pharmacy           

  ST LUKES               St. Luke's        
                         Medical            

  SAINT LUKES            St. Luke's        
                         Medical            

  MAKATI MEDICAL         Makati Medical     
                         Center             

  MAKATI MED             Makati Medical     
                         Center             

  THE MEDICAL CITY       The Medical City   

  MEDICAL CITY           The Medical City   

  ASIAN HOSPITAL         Asian Hospital     

  MAXICARE               Maxicare           HMO premium

  PHILCARE               PhilCare           HMO premium

  MEDICARD               MediCard           HMO premium

  INTELLICARE            Intellicare        HMO premium

  ANYTIME FITNESS        Anytime Fitness    

  FITNESS FIRST          Fitness First      

  GOLDS GYM              Gold's Gym        

  F45                    F45 Training       

  CULT FITNESS           Cult.fit           

  SNAP FITNESS           Snap Fitness       

  SLIMMER'S WORLD       Slimmer's World   
  ---------------------- ------------------ -----------------------------

**Seed List --- Housing (Essential)**

Housing is the hardest category to match from transaction descriptions
because rent payments often appear as direct bank transfers with no
merchant name. The rule-based lookup can only catch named property
management companies and known condo associations.

  ---------------------- ------------------ -----------------------------
  **Pattern**            **Merchant Name**  **Notes**

  AYALA PROPERTY         Ayala Land         

  SMDC                   SMDC               SM Development Corp

  MEGAWORLD              Megaworld          

  ROBINSONS LAND         Robinsons Land     

  DMCI HOMES             DMCI Homes         

  DMCI                   DMCI Homes         

  VISTA LAND             Vista Land         

  FED LAND               Federal Land       

  FEDERAL LAND           Federal Land       

  CONDO DUES             Condo Association  Generic pattern
                         Dues               

  ASSOCIATION DUES       Association Dues   

  HOA DUES               HOA Dues           

  RENTAL PAYMENT         Rent               Generic pattern for bank
                                            transfers

  RENT                   Rent               Use with caution --- also
                                            matches CURRENT
  ---------------------- ------------------ -----------------------------

| **Housing LLM dependency note:**                                      |
|                                                                       |
| Most rent payments in the Philippines are peer-to-peer bank transfers |
| that contain only a reference number or the landlord's name. These   |
| will not match any rule-based pattern. They will be passed to the     |
| OpenAI fallback, which should be prompted to look for keywords like   |
| "RENT", "RENTAL", "BOARDING", "ROOM", "BED SPACE" in the    |
| description and to classify recurring large transfers to individuals  |
| as likely Housing.                                                    |

**Seed List --- Transfers & Remittances (Committed)**

  ---------------------- ------------------ -----------------------------
  **Pattern**            **Merchant Name**  **Notes**

  GCASH SEND MONEY       GCash Transfer     P2P within GCash

  GCASH TRANSFER         GCash Transfer     

  SEND MONEY             GCash Transfer     GCash export label

  INSTAPAY               InstaPay Transfer  Bank-to-bank real-time

  PESONET                PESONet Transfer   Bank-to-bank batch

  PALAWAN EXPRESS        Palawan Express    

  PALAWAN PAWNSHOP       Palawan Pawnshop   

  LBC EXPRESS            LBC Express        

  WESTERN UNION          Western Union      

  MONEYGRAM              MoneyGram          

  REMITLY                Remitly            

  M LHUILLIER            M Lhuillier        

  MLHUILLIER             M Lhuillier        

  CEBUANA LHUILLIER      Cebuana Lhuillier  

  CEBUANA                Cebuana Lhuillier  

  COINS PH               Coins.ph           Crypto/remittance

  WIRE TRANSFER          Wire Transfer      International

  SWIFT                  SWIFT Transfer     International wire
  ---------------------- ------------------ -----------------------------

**Seed List --- Entertainment & Subscriptions (Discretionary)**

  ---------------------- ------------------ -----------------------------
  **Pattern**            **Merchant Name**  **Notes**

  DISNEY PLUS            Disney+            

  DISNEYPLUS             Disney+            

  DISNEY+                Disney+            

  HBO GO                 HBO Go             

  MAX                    Max (HBO)          Short --- check context

  VIVAMAX                Vivamax            Philippine streaming

  IQIYI                  iQIYI              

  AMAZON PRIME           Amazon Prime       

  PRIME VIDEO            Amazon Prime       

  STEAM                  Steam              PC gaming

  PLAYSTATION STORE      PlayStation Store  

  PS STORE               PlayStation Store  

  XBOX                   Xbox Game Pass     

  NINTENDO               Nintendo eShop     

  MOBILE LEGENDS         Mobile Legends     

  MOONTON                Mobile Legends     Developer name

  SUPERCELL              Clash of Clans     

  GARENA                 Garena (Free Fire) 

  SM CINEMA              SM Cinema          

  SM CINEMAS             SM Cinema          

  AYALA CINEMAS          Ayala Malls Cinema 

  TICKETNET              TicketNet          Events ticketing

  SMTICKETS              SM Tickets         Events ticketing

  TICKET2ME              Ticket2Me          Local events

  KLOOK                  Klook              Activities and experiences

  AGODA                  Agoda              Travel --- if recurring,
                                            classify Subs

  CANVA                  Canva              Design subscription

  ADOBE                  Adobe              Creative subscription
  ---------------------- ------------------ -----------------------------

**Seed List --- Miscellaneous (Catch-all)**

These patterns are classified as Miscellaneous because they cannot be
reliably attributed to a single category without more context. They are
always shown first in the "Does this look right?" correction list.

  ---------------------- ------------------ -----------------------------
  **Pattern**            **Merchant Name**  **Notes**

  ATM                    ATM Withdrawal     Cash --- unknown downstream
                                            use

  WITHDRAWAL             Cash Withdrawal    

  POS PURCHASE           POS Purchase       Generic point-of-sale --- no
                                            merchant info

  DEBIT PURCHASE         Debit Purchase     Generic

  ONLINE PURCHASE        Online Purchase    Generic --- LLM likely needed

  PAYMENT                Payment            Too generic --- always send
                                            to LLM

  GCASH QR               GCash QR Payment   QR code --- merchant not
                                            captured in description
  ---------------------- ------------------ -----------------------------

**2B.6 Stage 4 --- OpenAI Fallback**

Transactions that do not match any rule-based pattern are batched and
sent to OpenAI's API (model: gpt-4o-mini) for categorisation. The LLM
is not used for matched transactions --- rule-based results are treated
as ground truth.

```
// Batch structure sent to OpenAI const systemPrompt = ` You are a
  financial transaction categoriser for users in the Philippines. Given a
  list of bank transaction descriptions, assign each to exactly one
  category from the provided list. Return only valid JSON. Categories: -
  food_dining (restaurants, food delivery, coffee shops) - groceries
  (supermarkets, wet market, convenience stores) - bills_utilities
  (electricity, water, internet, phone, streaming bills) - transport
  (ride-hailing, fuel, transit top-up, toll) - shopping (retail, online
  marketplace, clothing, electronics) - health_wellness (pharmacy,
  clinic, hospital, HMO premium, gym) - housing (rent, condo dues, HOA,
  property management) - transfers (money transfers, remittances,
  padala) - entertainment_subs (streaming, gaming, events, activities) -
  misc (cannot determine --- use sparingly) Rules: 1. Philippine context:
  Grab Food = food_dining, Angkas = transport, Mercury Drug =
  health_wellness, Palawan Express = transfers 2. If description contains
  a person's name + amount, classify as transfers 3. Large one-time
  amounts (>₱10,000) to individuals: likely housing (rent) 4. ATM
  withdrawals: always misc 5. Confidence below 0.7: still assign best
  guess, set confidence accordingly `; const userPrompt = ` Categorise
  these transactions. Return JSON array: [ {"id": "tx_001",
  "category": "food_dining", "confidence": 0.95}, \... ]
  Transactions: ${unresolvedBatch.map(t => `${t.id}:
  "${t.normDesc}" ₱${t.amount}`).join("\\n")} `
```

  --------------- ---------------- ---------------------------------------
  **Parameter**   **Value**        **Reason**

  Model           gpt-4o-mini      Cost-efficient for batch
                                   classification. Accuracy sufficient for
                                   category-level assignment.

  Batch size      Max 50           Balances latency vs API cost.
                  transactions per Parallelise multiple batches.
                  API call         

  Temperature     0                Deterministic output --- categorisation
                                   is not a creative task

  Max tokens      200 per          JSON output is compact
                  transaction in   
                  batch            

  Timeout         10 seconds       If LLM times out: classify all
                                   timed-out transactions as misc, flag
                                   for correction

  Retry           1 retry on       Do not retry indefinitely --- degrade
                  failure          gracefully to misc
  --------------- ---------------- ---------------------------------------

**2B.7 Stage 5 --- Aggregation & Confidence Scoring**

```
type CategoryBreakdown = { [key: CategoryKey]: { monthlyAverage:
  number // average across N months monthlyAmounts: number[] // one per
  month (for trend analysis) tier: "essential" \| "discretionary" \|
  "committed" topMerchants: MerchantSummary[] transactionCount:
  number } } type CorrectionCandidate = { transaction: Transaction
  assignedCategory: CategoryKey confidence: number // 0--1; rule-based =
  1.0 reason: string // "rule_match" \| "llm" \| "llm_low_conf" \|
  "misc_fallback" }
```

Correction candidates shown to user (Screen 5, "Does this look
right?"):

-   All transactions where confidence < 0.7

-   All transactions classified as misc by fallback (not rule-matched)

-   ATM withdrawals (always shown --- user may want to reclassify)

-   Transfers to individuals > ₱5,000 (may be rent --- user should
    confirm)

-   Capped at 15 items displayed. Remaining low-confidence items
    accessible via "See all [N] flagged transactions" link.

**2B.8 Category Tiers**

Every category is tagged with a tier. Tiers are used internally for
scenario generation rules --- they are not shown as labels in the main
UI.

  ------------------ --------------- ----------------------------------------
  **Category**       **Tier**        **Scenario generation rule**

  Food & Dining      Discretionary   Generate SPENDING_CUT scenarios. Surface
                                     in Fastest Win calculation.

  Shopping           Discretionary   Generate SPENDING_CUT scenarios.

  Entertainment &    Discretionary   Generate SPENDING_CUT scenarios. Often
  Subscriptions                      quickest win (subscription
                                     cancellation).

  Groceries & Market Essential       No cut scenarios generated. Shown in
                                     burn breakdown only.

  Bills & Utilities  Essential       No cut scenarios. Bills are fixed
                                     commitments.

  Transport          Essential       No cut scenarios. Base commute is fixed.
                                     (Ride-hailing vs commute distinction not
                                     modelled in v1.)

  Health & Wellness  Essential       Never generate cut scenarios. Cutting
                                     health spend is never surfaced as a win.

  Housing            Essential       HOUSING_CHANGE scenario only ---
                                     user-input delta, never a cut
                                     suggestion.

  Transfers &        Committed       Never generate cut scenarios.
  Remittances                        Acknowledged in diagnosis copy if > 15%
                                     of burn.

  Miscellaneous      Discretionary   No scenarios generated. Shown as a line
                                     item. User encouraged to review.
  ------------------ --------------- ----------------------------------------

### 3. Zone System

### 3.1 Zone Definitions

Every runway score maps to one of four zones. Zones determine: the
colour of the runway display, the zone callout text, and which Sprout
Finances product is shown on Screen 8.

  ---------- ---------- --------------- ------------------------------------
  **Zone**   **Days**   **Colour        **Plain-language meaning**
                        Token**         

  Critical   < 30      TOMATO (red)    Savings cover less than a month.
                                        Focus on one action right now.

  Fragile    30 -- 59   MANGO (yellow)  About 1--2 months of runway. One
                                        unexpected expense could strain you.

  Stable     60 -- 119  BLUEBERRY       2--4 months of breathing room.
                        (blue)          Enough to handle most surprises ---
                                        but not enough to stop watching.

  Strong     120+       KANGKONG        4+ months of cushion.
                        (green)         Well-positioned --- the goal now is
                                        to make this money work harder.
  ---------- ---------- --------------- ------------------------------------

Alex's baseline: 103 days → Stable Zone.

### 3.2 Zone Callout (Screen 6)

Below the zone gradient bar, a callout card shows the user's current
zone in plain English. It updates live as scenarios are toggled. It does
not show zone definitions for other zones --- only the zone the user is
currently in.

| **Example --- Stable Zone callout:**                                  |
|                                                                       |
| *"You're in the Stable Zone · 60--119 days"*                       |
|                                                                       |
| "2--4 months of breathing room. Enough to handle most surprises ---  |
| but not enough to stop watching."                                    |

### 3.3 Zone-to-Product Routing

Screen 8 shows exactly one Sprout Finances product card, selected by
zone:

  ---------- --------------- ---------------------------------------------
  **Zone**   **Product       **Logic**
             Shown**         

  Critical   ReadyWage       Immediate access to earned wages --- fastest
                             path to cash

  Fragile    ReadyCash       Interest-free emergency loan --- bridge the
                             gap

  Stable     ReadySave       Build buffer while there is still room

  Strong     ReadySave       Optimise --- lock in growth before lifestyle
                             creep catches up
  ---------- --------------- ---------------------------------------------

The product shown on Screen 8 reflects the user's score at the time
they tap "What's my next move?" --- if they toggled scenarios on
Screen 6 and moved zones, Screen 8 shows the product for the new zone.

### 4. Computation Model

### 4.1 Core Formula

```
runwayDays = Math.floor(liquidCash / (monthlyBurn / 30)) where:
  liquidCash = user-entered savings (all accounts combined), in ₱
  monthlyBurn = average monthly spend from transaction data, in ₱ 30 =
  days per month (fixed constant --- do not use calendar months)
```

### 4.2 Monthly Burn Calculation

Monthly burn is the average of the last N complete months of categorised
spend (N = 3 by default, configurable up to 6).

```
monthlyBurn = sum(categoryMonthlyAverages)
  categoryMonthlyAverage[cat] = sum(transactions[cat] for each
  complete month) / numberOfMonths // Each category is averaged
  independently before summing // Incomplete current month is excluded
  from averaging // Months with zero spend in a category are included in
  the divisor
```

Demo values (4 months Jul--Sep 2024):

  ------------------------------------- ---------------------------------
  **Category**                          **Monthly Average**

  Food & Dining                         ₱18,200

  Groceries                             ₱9,400

  Bills & Utilities                     ₱8,100

  Transport                             ₱7,600

  Shopping                              ₱5,800

  Entertainment & Subs                  ₱3,300

  Total Monthly Burn                    ₱52,400
  ------------------------------------- ---------------------------------

### 4.3 Scenario Computation Model

Scenarios are typed objects. Deltas are never hardcoded --- they are
always computed from the user's actual data at runtime.

### 4.3.1 Scenario Types

```
// Scenario type definitions SPENDING_CUT // Reduce a specific category
  by % or fixed ₱ INCOME_GAIN // Recurring monthly income increase
  ONE_TIME_INJECT // Lump-sum addition to liquid cash (no burn change)
  HOUSING_CHANGE // Rent delta --- requires user input (no transaction
  source) CUSTOM // User-defined signed monthly amount (+ = income, - =
  cut)
```

### 4.3.2 Scenario Object Shape

```
type Scenario = { id: string // unique, stable type: ScenarioType //
  one of the 5 types above label: string // shown on chip effort:
  "quick" \| "habit" \| "life" recurrence: "one-time" \|
  "recurring" params: ScenarioParams // type-specific --- see below
  assumption?: string // shown when params use estimate, not data }
```

### 4.3.3 ScenarioParams by Type

```
// SPENDING_CUT { category: CategoryKey, cutPct?: number, cutAmount?:
  number } // cutPct and cutAmount are mutually exclusive // cutPct: 0--1
  (e.g. 0.70 = 70% reduction) // cutAmount: fixed ₱ reduction per month
  // INCOME_GAIN { gainAmount: number } // ₱ per month, recurring //
  ONE_TIME_INJECT { injectAmount: number } // ₱ lump sum added to
  liquidCash // HOUSING_CHANGE { rentDelta: number } // ₱/month ---
  positive = more expensive, negative = cheaper // rentDelta is
  user-inputted when assumption is active // defaultAssumption:
  +₱15,000/month if user does not enter // CUSTOM { monthlyAmount:
  number, userLabel: string } // positive = income gain (reduces monthly
  gap) // negative = spending cut (reduces burn)
```

### 4.3.4 Delta Computation --- Pure Functions

All delta computations are pure functions of (scenario, state). State is
immutable per computation. The functions must be fully unit-testable
with no side effects.

```
type RunwayState = { liquidCash: number // ₱ monthlyBurn: number //
  ₱/month takeHome: number // ₱/month net salary categories:
  Record<CategoryKey, number> // monthly avg per category } function
  computeBaseline(state: RunwayState): number { return
  Math.floor(state.liquidCash / (state.monthlyBurn / 30)) } function
  computeScenarioDays(scenario: Scenario, state: RunwayState): number {
  switch (scenario.type) { case "SPENDING_CUT": { const catSpend =
  state.categories[scenario.params.category] const reduction =
  scenario.params.cutAmount ?? (catSpend * scenario.params.cutPct) const
  newBurn = state.monthlyBurn - reduction return
  Math.floor(state.liquidCash / (newBurn / 30)) } case "INCOME_GAIN": {
  // Extra income reduces monthly gap (how fast savings drain) const
  monthlyGap = state.monthlyBurn - state.takeHome const newGap =
  Math.max(0, monthlyGap - scenario.params.gainAmount) const
  effectiveBurn = state.takeHome + newGap return
  Math.floor(state.liquidCash / (effectiveBurn / 30)) } case
  "ONE_TIME_INJECT": { const newCash = state.liquidCash +
  scenario.params.injectAmount return Math.floor(newCash /
  (state.monthlyBurn / 30)) } case "HOUSING_CHANGE": { const newBurn =
  state.monthlyBurn + scenario.params.rentDelta return
  Math.floor(state.liquidCash / (newBurn / 30)) } case "CUSTOM": {
  const { monthlyAmount } = scenario.params const newBurn = monthlyAmount
  < 0 ? state.monthlyBurn + monthlyAmount // spending cut :
  state.monthlyBurn - monthlyAmount // income gain reduces drain return
  Math.floor(state.liquidCash / (Math.max(1, newBurn) / 30)) } } }
  function computeDelta(scenario: Scenario, state: RunwayState): number {
  return computeScenarioDays(scenario, state) - computeBaseline(state) }
```

| **YAGNI boundary**                                                    |
|                                                                       |
| Do not implement stacked scenario computation as a recursive          |
| function. Active scenarios are applied independently against the      |
| baseline state, not sequentially. Combined delta = sum of individual  |
| deltas. This is intentionally simplified for v1. Compound             |
| interactions (e.g. cutting food AND getting a raise simultaneously)   |
| are not modelled beyond additive combination.                         |

### 5. Scenario Playground

### 5.1 Overview

The Scenario Playground is the interactive core of Runway. It is not a
generic what-if calculator --- it generates personalised scenarios from
the user's actual transaction data, tags them by effort, and offers a
reverse mode that solves for a target.

### 5.2 Predefined Scenarios (Data-Derived)

Pre-authored chips are generated at runtime from transaction data, not
hardcoded. The labels, cut percentages, and baseline amounts are all
derived from the user's actual numbers.

  ------------- ---------------- --------------------------- ---------- -------------------
  **Chip        **Type**         **Computation**             **Effort   **Assumption (if
  Label**                                                    Tag**      any)**

  "Return      SPENDING_CUT     cutAmount =                 quick      None --- uses
  [top                          currentMonthTopMerchant -              actual data
  merchant] to                  baselineMonthTopMerchant.              
  [month]                      Category = Food & Dining.              
  levels"                       Label uses actual merchant             
                                 name and month.                        

  "Cut dining  SPENDING_CUT     cutPct = 0.70 on Food &     habit      None --- uses
  & subs 70%"                   Dining + Entertainment.                actual data
                                 newBurn computed from                  
                                 actual category averages.              

  "Salary      INCOME_GAIN      gainAmount = takeHome ×     life       Assumes raise takes
  raise 10%"                    0.10. Uses actual net                  effect immediately
                                 take-home from payroll.                

  "Side hustle INCOME_GAIN      gainAmount = 10000. Fixed   habit      Assumes sustained
  ₱10k/mo"                      assumption --- not data                ₱10,000/month
                                 derived.                               additional income

  "Move to     HOUSING_CHANGE   rentDelta = user input      life       Default
  bigger unit"                  (inline). Default                      +₱15,000/month.
                                 assumption = +₱15,000/month            Shown on chip:
                                 if user does not enter.                "assumes +₱15k/mo
                                                                        rent increase"
  ------------- ---------------- --------------------------- ---------- -------------------

| **Data-derived chip generation rule**                                 |
|                                                                       |
| generateFromTransactions(transactions, state) must produce the full   |
| scenario list. Each call re-derives labels and amounts from current   |
| data. If a category has zero spend, skip that scenario. If top        |
| merchant is the same as the baseline month, skip the merchant-level   |
| chip and show only the category chip.                                 |

### 5.3 Effort Tagging

  ------------ ------------------------------- ---------------------------
  **Tag**      **Definition**                  **Scenarios**

  Quick win    Can be acted on today. No       Cancel a subscription,
               sustained behaviour change      access ReadyWage
               required.                       

  Habit change Requires sustained behaviour    Cut dining, side hustle
               shift over weeks/months.        income

  Life change  Structural decision. Takes      Salary raise, move
               months or longer to execute.    apartments
  ------------ ------------------------------- ---------------------------

In v1, effort tags are displayed as a small label on the chip (not a
filter). The filter UI is deferred --- tag the data now, expose the
filter in v2.

### 5.4 Fastest Win Banner

Above the chips, a persistent banner highlights the single scenario that
gives the most days with the lowest effort tag:

```
fastestWin = scenarios .filter(s => s.effort === "quick" \|\|
  s.effort === "habit") .sort((a, b) => computeDelta(b, state) -
  computeDelta(a, state)) [0] // Banner: "Your fastest win: [label]
  --- adds [delta] days with one habit change"
```

### 5.5 Reverse Mode ("What Would It Take?")

A secondary toggle at the top of the playground switches to reverse
mode. The user sets a target runway (e.g. 120 days) and the engine
computes the minimum combination of changes needed to reach it.

```
function findFastestPath( targetDays: number, state: RunwayState,
  scenarios: Scenario[] ): Scenario[] { // 1. Filter to scenarios
  with positive delta const positive = scenarios .filter(s =>
  computeDelta(s, state) > 0) .sort((a, b) => computeDelta(b, state) -
  computeDelta(a, state)) // 2. Greedily select highest-delta scenarios
  until target is met const selected: Scenario[] = [] let current =
  computeBaseline(state) for (const s of positive) { if (current >=
  targetDays) break selected.push(s) current += computeDelta(s, state) }
  return selected } // UI: pre-select the returned scenarios as active
  chips // Show: "To reach 120 days, you need: [chip 1] + [chip 2]"
```

| **YAGNI: greedy algorithm only in v1**                                |
|                                                                       |
| Do not implement optimal subset-sum pathfinding. The greedy approach  |
| (pick highest-delta first) is good enough and fast. True optimisation |
| can be added later if user research shows it matters.                 |

### 5.6 Stacked Result Display

When one or more chips are active, the stacked result panel shows:

-   New runway in days (large number)

-   Delta from baseline (e.g. "+19 days") coloured by direction

-   Zone badge for new zone

-   "Your savings last until: [calendar date]" --- computed as
    today + newDays

```
// Calendar date translation function daysToDate(days: number,
  referenceDate: Date = new Date()): string { const d = new
  Date(referenceDate) d.setDate(d.getDate() + days) return
  d.toLocaleDateString("en-PH", { month: "long", day: "numeric",
  year: "numeric" }) } // referenceDate should be the date the analysis
  was run, not today() // This prevents the date shifting on return
  visits
```

### 5.7 Custom Scenario --- Inline Expander

The "+ Custom scenario" chip opens an inline expander directly below
the chips row (not a modal). Spec:

-   Two fields: Label (text, max 20 chars) and Monthly Change (signed
    number in ₱)

-   Positive amount = income gain. Negative amount = spending cut.

-   Apply button or Enter key confirms. Cancel collapses the expander
    and removes any applied delta.

-   After apply, the chip label shows the user's entered label,
    truncated to 20 chars. Delta badge shows calculated days.

-   Computation: uses CUSTOM scenario type. See 4.3.3.

-   Enter in Label field: focuses Amount field. Enter in Amount field:
    triggers Apply.

| **Validation rule**                                                   |
|                                                                       |
| -   Amount = 0 or empty: show field error, do not apply               |
|                                                                       |
| -   Amount out of range (> ₱500,000): show warning "That's larger  |
|     than most monthly budgets --- double-check your number"          |
|                                                                       |
| -   No label entered: use "Custom" as fallback                      |

### 6. Screen Specifications

8 screens, all interactive. Components use TOGE design system tokens
throughout. See Section 7 for component mapping.

**Screen 1 --- Entry Point (Pay Feed)**

  --------------- -------------------------------------------------------
  **Element**     **Spec**

  Context         My Pay feed. Payslip card is the primary element.
                  Runway teaser is a contextual strip attached to the
                  bottom of the payslip card.

  Payslip card    Shows net pay (₱28,500), gross, deductions, loan
                  deduction. "View Details" secondary button.

  Runway teaser   Green tinted background
  strip           (spr-background-color-brand-weak). Sprout logo mark,
                  headline "What's your financial runway?", sub-line,
                  CTA button.

  CTA             spr-button tone="success" variant="primary"
                  label="Check My Runway →"

  Entry condition Show once per payday cycle. If user has a saved score:
                  show "Your Runway: X days" chip instead of CTA.
  --------------- -------------------------------------------------------

**Screen 2 --- Payroll Profile**

  --------------- -------------------------------------------------------
  **Element**     **Spec**

  Header          "Your payroll, ready to go" + sub-line: "We
                  pre-filled this from your September 30 payslip. Just
                  confirm your savings below."

  Payroll data    spr-card, plain tone. Rows for each payroll field.
  card            spr-badge variant="brand" label="Pre-filled by
                  Sprout" in header.

  Consent block   spr-card tone="information". Explains that payroll
                  data stays within Sprout. Two buttons: "I understand,
                  continue →" (primary info tone) and "Not now"
                  (tertiary).

  Savings input   spr-input type="text" with ₱ prefix. Label: "How
                  much do you have in savings right now?" Supporting
                  text: "All bank accounts and e-wallets combined. This
                  stays on your device."

  CTA             "Continue →" spr-button tone="success"
                  variant="primary" fullwidth
  --------------- -------------------------------------------------------

**Screen 3 --- Data Connection**

  --------------- -------------------------------------------------------
  **Element**     **Spec**

  Header          "Choose how to analyze your spending" + sub-line:
                  "The more complete your data, the more accurate your
                  number."

  Tier 1 card     GCash connect. "Recommended" tier-badge. spr-badge
                  variant="brand" "Most accurate". Primary CTA:
                  spr-button tone="success" fullwidth "Connect GCash"

  Tier 2 card     CSV upload. Dashed drop zone. "Choose File" secondary
                  button.

  Tier 3 card     Estimate. spr-input with ₱ prefix for monthly spend.
                  "Use Estimate →" secondary button.

  Selection       Tapping a card highlights it with a brand-tone border.
                  Only one tier active at a time.
  --------------- -------------------------------------------------------

**Screen 4 --- Processing**

  --------------- -------------------------------------------------------
  **Element**     **Spec**

  Header          "Analyzing your transactions" + sub-line showing data
                  period.

  Progress bar    spr-progress-bar color="success" size="sm". Fills
                  as categories appear. Shows % complete.

  Category rows   Each category fades in sequentially (400ms delays).
                  Icon, merchant list snippet, spr-badge with amount.

  Auto-advance    After all categories visible + 800ms pause, advance to
                  Screen 5 automatically.
  --------------- -------------------------------------------------------

**Screen 5 --- Intelligence Report**

  --------------- -------------------------------------------------------
  **Element**     **Spec**

  Header          "Here's where your money goes" + "Based on 4 months
                  of transaction data, averaged per month."

  Burn breakdown  spr-card plain. One row per category with
  card            spr-progress-bar size="sm" and monthly amount. Final
                  row: total burn in large danger-toned text.

  Progress bar    width% = (categoryAmount / totalBurn) × 100. Cap at 95%
  widths          for visual cleanliness.

  Danger signals  Up to 3 signals. Each is a card with spr-status (danger
                  or caution), description, and spr-badge with metric.
                  Signals are data-derived --- only shown if threshold is
                  met.

  Danger signal 1 Any category growing > 25% in the last 3 months.
  trigger         Label: "[Category] growing fast. +X% in 3 months."

  Danger signal 2 Monthly burn > net take-home. Label: "Monthly spend
  trigger         exceeds take-home pay. ₱X gap."

  CTA             "Get My Breakdown →" advances to Screen 6.

  "Does this     Collapsed section at bottom of Screen 5. Trigger: "We
  look right?"   categorised [N] transactions. Anything look off? ↓".
                  Expands to show top 15 correction candidates (see
                  Section 2B.7). Each row shows: merchant/description,
                  amount, assigned category chip, and a reassign tap
                  target. Full list accessible via "See all [N]
                  flagged" link. User corrections update the burn
                  breakdown and runway number in real time.
  --------------- -------------------------------------------------------

**Screen 6 --- Survival Dashboard**

  --------------- -------------------------------------------------------
  **Element**     **Spec**

  Runway number   Large number (font-size 88px, font-weight 800). Colour
                  = zone colour token. Updates live on scenario toggle.

  Zone bar        CSS gradient (TOMATO → MANGO → BLUEBERRY → KANGKONG).
                  Zone marker dot (18px circle, white fill, zone-coloured
                  border) positioned at (days / 160) × 100%.

  Zone callout    Below the bar. Background, border, and title colour
                  match current zone. Updates live. Shows current zone
                  description only.

  Fastest win     Above scenario chips. Star icon. "Your fastest win:
  banner          [label] --- adds [X] days with one habit change."

  Scenario chips  spr-chips component. Each chip shows label + delta
                  badge. Active state: green background. Negative delta
                  chips (e.g. Move apartments) show red delta badge.

  Stacked result  Shown when ≥ 1 chip active. New days, delta, zone
                  badge, calendar date.

  Reverse mode    "Set a target →" text button above chips. Opens
  toggle          target input: number field for desired days. On submit,
                  pre-selects the greedy-optimal chip set.

  Custom scenario "+ Custom scenario" chip with dashed border. On tap:
                  inline expander with label + amount fields. See Section
                  5.7.

  CTA             "What's My Next Move? →" spr-button tone="success"
                  variant="primary"
  --------------- -------------------------------------------------------

**Screen 7 --- Behavioral Diagnosis**

  --------------- -------------------------------------------------------
  **Element**     **Spec**

  Pattern header  Dark green gradient card (not spr-card --- custom).
                  Shows "Your Pattern" label, archetype name, and
                  one-sentence description.

  Archetype       Four archetypes: Lifestyle Inflator (burn > income,
  system          growing), Steady Spender (stable burn, matches income),
                  Resilient Saver (burn < income, building), Crisis Mode
                  (burn >> income, depleting fast). Determined by burn
                  trend + gap size.

  Diagnosis body  spr-card plain. Three blocks: "What's happening"
                  (data-specific sentences using actual numbers), "What
                  to do about it" (top recommended action with
                  before/after runway), "The honest take" (italic,
                  plain-language summary).

  "What to do"  Shows the fastest win scenario from Section 5.4 as a
  block           concrete recommendation. Before/after runway displayed
                  as two numbers with arrow.

  CTA             "Get My Breakdown →" advances to Screen 8. Note:
                  "Get my breakdown" from Screen 5 advances to Screen
                  6. The Screen 7 CTA is "What's My Next Move? →"
  --------------- -------------------------------------------------------

| **Correction from prototype**                                         |
|                                                                       |
| Screen 7 CTA is "What's My Next Move? →" (not "Get My Breakdown   |
| →"). "Get My Breakdown →" is the Screen 5 CTA that takes the user  |
| from the burn report to the runway dashboard.                         |

**Screen 8 --- Action Card**

  --------------- -------------------------------------------------------
  **Element**     **Spec**

  Problem card    Zone-coloured card. Badge with zone name + current
  (A1)            days. Problem statement: personalised to user's burn,
                  savings, and days. Product bridge line. All three
                  elements update dynamically based on current score.

  Problem         Critical: "At ₱X/mo burn, your ₱Y buffer runs out in Z
  statements      days. You need breathing room today." Fragile: "Your
                  savings cover Z days. One unexpected expense could push
                  you into the red." Stable: "At ₱X/mo burn, your ₱Y
                  buffer runs out in Z days. The goal is to grow this
                  before your spending pattern closes the gap." Strong:
                  "At Z days, your buffer is solid. The opportunity now
                  is to make this money work harder while you have
                  room."

  Product card    One card shown, determined by zone. See Section 3.3.

  Before/after    ReadySave: shows 103 days → 117 days (at
  projection      ₱2,000/payday). ReadyCash/ReadyWage: shows current days
                  → days after ₱10,000 injection.

  Amount chips    spr-chips row: ₱1,000 / ₱2,000 / ₱3,000 / ₱5,000.
  (ReadySave)     Active chip updates CTA text and projection. Default:
                  ₱2,000.

  CTA copy (C3)   ReadySave stable: "Lock in ₱X per payday --- starting
                  [next payday]". ReadySave strong: "Put my buffer to
                  work --- starting [next payday]". ReadyCash:
                  "Bridge the Gap with ReadyCash". ReadyWage: "Get My
                  Earned Wages Now".

  Completion      After CTA tap: product card fades to 45% opacity.
  state           Success card appears with green checkmark, "Your
                  runway is saved", current score, and link back to Pay
                  Feed.
  --------------- -------------------------------------------------------

### 7. TOGE Design System --- Component Mapping

Runway uses the Sprout TOGE design system (design-system-next, Vue 3 +
TypeScript). All components are prefixed spr-. Below is the complete
component usage map for the Runway feature.

  ------------------ ---------------------- ------------------------------------
  **Component**      **Props Used**         **Where / Notes**

  spr-button         tone, variant, size,   All CTAs. See tone guide below.
                     fullwidth              

  spr-card           tone, show-header,     Primary content container
                     has-content-padding,   throughout.
                     border-radius-size     

  spr-chips          :active, \@update,     Scenario chips on Screen 6. Amount
                     tone, closable         selector on Screen 8.

  spr-progress-bar   value, max, color,     Processing screen (Screen 4), burn
                     size                   breakdown (Screen 5).

  spr-badge          variant, size          Inline labels throughout. Variants:
                                            brand, information, danger, pending,
                                            caution, neutral, accent.

  spr-status         state, size            Danger signals on Screen 5. state:
                                            success \| information \| pending \|
                                            caution \| danger.

  spr-input          type, label,           Savings input (Screen 2), category
                     supporting-label,      estimate (Screen 3), custom scenario
                     modelValue, prefix     (Screen 6), reverse mode target
                     slot                   (Screen 6).

  spr-collapsible    :modelValue,           Full diagnosis text on Screen 7
                     \@update:modelValue    (collapsed by default, expands
                                            inline).

  spr-logo           name, theme, width     ReadyCash, ReadyWage, ReadySave
                                            logos on Screen 8. theme="white"
                                            on dark backgrounds.
  ------------------ ---------------------- ------------------------------------

### 7.1 Button Tone Guide

  -------------------------- ------------- ------------------------------
  **Context**                **Tone**      **Variant**

  Primary CTA (advance,      success       primary
  connect, save)                           

  Secondary CTA (back, learn neutral       secondary
  more)                                    

  Destructive or skip        neutral       tertiary

  Information actions        neutral       secondary
  (consent continue)                       
  -------------------------- ------------- ------------------------------

### 7.2 Colour Tokens Used

  ---------------- ------------- ---------------- -----------------------
  **Semantic       **Colour      **Token Range    **Example Usage**
  Role**           Name**        Used**           

  Brand / Success  KANGKONG      50, 100, 200,    Primary CTAs, Stable
                                 600, 700, 800    zone, savings goals

  Danger           TOMATO        50, 100, 200,    Critical zone, burn
                                 600, 700         rate warnings

  Information      BLUEBERRY     50, 100, 200,    Stable zone callout,
                                 600, 700, 800    consent banners

  Pending          MANGO         50, 100, 200,    Fragile zone,
                                 500, 600         processing state

  Caution          CARROT        100, 500         Caution signals,
                                                  secondary warnings

  Neutral          MUSHROOM      50--950          All text, borders,
                                                  backgrounds

  Accent           WINTERMELON   100, 600         Entertainment category
                                                  chip
  ---------------- ------------- ---------------- -----------------------

### 7.3 Typography Scale Used

  ------------------- ----------- ----------------------------------------
  **Token**           **Size**    **Usage in Runway**

  spr-heading-sm      28px / 500  Screen headers (h1 equivalent per
                                  screen)

  spr-heading-xs      24px / 500  Archetype name on Screen 7

  spr-subheading-sm   20px / 500  Net pay display on payslip

  spr-body-md         14px        Standard body text throughout

  spr-body-sm         13px        Card content, secondary info

  spr-body-xs         12px        Supporting labels, supporting text

  spr-label-xs        12px /      Step labels, section dividers
                      uppercase / 
                      700         
  ------------------- ----------- ----------------------------------------

### 8. Engineering Notes (Superpowers-Compatible)

This section contains the implementation constraints, TDD anchors, YAGNI
boundaries, and DRY rules for the Runway feature. An implementation plan
generated from this spec should follow these exactly.

### 8.1 Architecture Principles

-   YAGNI: Build only what is specced. Scenario conflict detection,
    multi-currency support, and employer-facing dashboards are
    explicitly out of scope for v1.

-   DRY: The computation model (Section 4) is the single source of truth
    for all runway number derivations. The UI never computes its own
    delta --- it always calls computeDelta().

-   TDD: All pure computation functions (computeBaseline,
    computeScenarioDays, computeDelta, findFastestPath,
    generateFromTransactions) must be unit-tested before UI integration.
    Red/green first.

### 8.2 Required Unit Tests

These are the minimum test cases. Each must be a true red/green test ---
test written before implementation.

  --------------------- ---------------------------- ------------------------
  **Function**          **Test Case**                **Expected**

  computeBaseline       liquidCash=180000,           103
                        monthlyBurn=52400            

  computeBaseline       liquidCash=0,                0
                        monthlyBurn=any              

  computeBaseline       liquidCash=any,              Infinity or
                        monthlyBurn=0                MAX_SAFE_INTEGER ---
                                                     document the choice

  computeScenarioDays   SPENDING_CUT 70% on          Math.floor(180000 /
                        food=18200, baseline         ((52400 - 12740) / 30))
                        burn=52400                   = 136

  computeScenarioDays   INCOME_GAIN 2850 (10% of     Math.floor(180000 /
                        28500), gap=23900            ((23900 - 2850 + 28500)
                                                     / 30)) = 107

  computeScenarioDays   ONE_TIME_INJECT 10000,       Math.floor(190000 /
                        cash=180000, burn=52400      (52400/30)) = 108

  computeScenarioDays   HOUSING_CHANGE +15000,       Math.floor(180000 /
                        burn=52400                   (67400/30)) = 80

  computeScenarioDays   CUSTOM -5000 (spending cut), Math.floor(180000 /
                        burn=52400                   (47400/30)) = 113

  computeDelta          Any scenario                 computeScenarioDays(s,
                                                     state) -
                                                     computeBaseline(state)

  daysToDate            days=103, ref=2024-09-30     "January 11, 2025"

  findFastestPath       target=120, baseline=103,    [+19 days scenario]
                        scenarios=[+19, +15, +6,    --- greedy stops after
                        -23]                        first

  getZone               29                           "critical"

  getZone               30                           "fragile"

  getZone               60                           "stable"

  getZone               120                          "strong"
  --------------------- ---------------------------- ------------------------

### 8.3 State Management

```
// RunwayStore --- single source of truth for the Runway flow type
  RunwayStore = { // Input state liquidCash: number transactions:
  Transaction[] // from GCash API or CSV analysisDate: Date // date
  analysis was run (frozen) // Computed state (derived, never manually
  set) state: RunwayState // computed from liquidCash + transactions
  baselineDays: number // computeBaseline(state) zone: ZoneName //
  getZone(baselineDays) categories: CategoryBreakdown dangerSignals:
  DangerSignal[] scenarios: Scenario[] //
  generateFromTransactions(transactions, state) // Scenario playground
  state activeScenarioIds: string[] customScenario: Scenario \| null
  reverseTarget: number \| null // Computed scenario output (derived)
  stackedDays: number stackedDelta: number stackedZone: ZoneName
  stackedDate: string }
```

### 8.4 YAGNI Boundaries --- Explicitly Out of Scope for v1

  -------------------------- --------------------------------------------
  **Feature**                **Reason Deferred**

  Scenario conflict          Adds complexity without clear user value
  detection                  until post-launch research confirms it
                             matters

  Employer-facing runway     Privacy implications need separate policy
  aggregate dashboard        review

  Multi-currency support     All users are Philippine-based. ₱ only.

  Scenario filter by effort  Tags are data-ready in v1. Filter UI ships
  tag                        in v2.

  Optimal subset-sum path    Greedy algorithm is sufficient. Optimality
  for reverse mode           difference is < 5% in realistic scenarios.

  Push notifications for     Notification permission flow adds scope.
  runway score changes       Show on next payslip instead.

  Savings goal tracking over ReadySave handles this. Runway does not
  time                       duplicate it.
  -------------------------- --------------------------------------------

### 8.5 DRY Rules

-   The zone colour map (ZONES object) is defined once and used by: the
    zone bar marker, the zone callout, the stacked result badge, and the
    Screen 8 problem card. Never duplicate it.

-   daysToDate() is the only place that converts days to a calendar
    string. Screen 6 stacked result and any future notification copy
    both call this function.

-   computeBaseline() is the only place that applies the runway formula.
    Screen 6 display, Screen 7 diagnosis copy, and Screen 8 problem card
    all derive their numbers from this function.

-   The Scenario type and computeDelta() live in a shared computation
    module. Neither the store nor the UI components contain computation
    logic.

### 8.6 Demo Mode

For the hackathon demo, the application runs in DEMO_MODE=true. The key
rule: only Agent 4 (Diagnosis Narrative) makes a live API call during
the demo. Agents 1, 2, and 3 return pre-computed fixture data. The
computation model runs on real data in both modes.

Full Demo Mode spec --- fixture data, Screen 4 AI visibility, Screen 7
attribution, latency targets --- is in Section 12.

| **Core Demo Mode rules (summary)**                                    |
|                                                                       |
| -   DEMO_MODE is a build-time constant. Not a runtime toggle.         |
|                                                                       |
| -   Screens 2 and 3: pre-fill Alex Garcia dataset regardless of path  |
|     chosen.                                                           |
|                                                                       |
| -   Agents 1, 2, 3: return fixture data from /src/demo/fixtures/. No  |
|     API calls.                                                        |
|                                                                       |
| -   Agent 4: live gpt-4o call with 10-second timeout and pre-written  |
|     fallback.                                                         |
|                                                                       |
| -   Computation model: always live. Scenario deltas always computed   |
|     from real state.                                                  |

### 9. Sprout Finances Product Cards

Screen 8 presents one of three Sprout Finances products depending on the
user's zone. Each card follows the A1 (problem-first) + C3
(zone-specific CTA) pattern.

### 9.1 ReadySave (Stable + Strong)

| **Product description (from Sprout Finances):**                       |
|                                                                       |
| Automatic payroll deduction that sets aside a fixed amount on every   |
| payday before the employee can spend it. Builds financial buffer      |
| systematically without requiring sustained behaviour change.          |

  ------------------ ----------------------------------------------------
  **Card Element**   **Content**

  Problem statement  "At ₱[monthlyBurn]/mo burn, your ₱[liquidCash]
  (Stable)           buffer runs out in [days] days. The goal is to
                     grow this before your spending pattern closes the
                     gap."

  Product bridge     "ReadySave turns every payday into runway."
  line               

  Single benefit     Before/after projection: "Now: [X] days → In 6
                     months: [X + gain] days" with days-gain badge

  Amount chips       ₱1,000 / ₱2,000 / ₱3,000 / ₱5,000 per payday.
                     Default: ₱2,000. Chips update CTA and projection.

  Days-gain per      ₱1,000 → +10 days. ₱2,000 → +14 days. ₱3,000 → +21
  amount             days. ₱5,000 → +35 days. (Computed as: amount × 12
                     paydays ÷ dailyBurn, rounded to nearest day)

  CTA (Stable)       "Lock in ₱[amount] per payday --- starting [next
                     payday date]"

  CTA (Strong)       "Put my buffer to work --- starting [next payday
                     date]"

  "Maybe later"    Tertiary neutral button. Records dismissal. Does not
  link               show again on this payday cycle.
  ------------------ ----------------------------------------------------

### 9.2 ReadyCash (Fragile)

| **Product description (from Sprout Finances):**                       |
|                                                                       |
| Interest-free emergency fund up to a pre-approved limit, disbursed    |
| directly to the employee's bank account in minutes. Repaid via       |
| automatic salary deduction. No paperwork, no queue, no company        |
| liability.                                                            |

  ------------------ ----------------------------------------------------
  **Card Element**   **Content**

  Problem statement  "Your savings cover [days] days. One unexpected
                     expense could push you into the red. It's time to
                     build a bridge."

  Product bridge     "ReadyCash gives you an interest-free bridge in
  line               minutes."

  Single benefit     Before/after days projection: current days → days
                     after ₱10,000 ReadyCash injection. Delta computed
                     via ONE_TIME_INJECT scenario.

  Supporting copy    "Interest-free. Pre-approved. No paperwork. Repaid
                     automatically via salary deduction."

  CTA                "Bridge the Gap with ReadyCash"

  "Maybe later"    Tertiary neutral button.
  ------------------ ----------------------------------------------------

### 9.3 ReadyWage (Critical)

| **Product description (from Sprout Finances):**                       |
|                                                                       |
| On-demand access to wages the employee has already earned but not yet |
| received. Available at any time, in any amount up to earned balance.  |
| No interest, no risk, no company liability. Balance replenishes daily |
| as the employee works.                                                |

  ------------------ ----------------------------------------------------
  **Card Element**   **Content**

  Problem statement  "At ₱[monthlyBurn]/mo burn, your ₱[liquidCash]
                     buffer runs out in [days] days. You need breathing
                     room today."

  Product bridge     "ReadyWage gets your earned wages into your account
  line               today."

  Context sentence   "You have earned wages sitting in payroll ---
                     available to you right now."

  Single benefit     Before/after days projection: current days → days
                     after ₱10,000 ReadyWage access. Delta computed via
                     ONE_TIME_INJECT.

  Supporting copy    "No interest. No risk. These are wages you've
                     already earned --- ReadyWage just lets you access
                     them early."

  CTA                "Get My Earned Wages Now"

  "Maybe later"    Tertiary neutral button.
  ------------------ ----------------------------------------------------

**9.4 spr-logo Implementation**

All product cards use spr-logo component from TOGE. Configuration:

```
<spr-logo name="readysave" theme="white" width="44" /> <!\--
  on dark hero \--> <spr-logo name="readycash" theme="white"
  width="44" /> <!\-- on dark hero \--> <spr-logo
  name="readywage" theme="white" width="44" /> <!\-- on dark hero
  \--> <spr-logo name="finances" theme="green" width="28" />
  <!\-- sidebar / feed \--> // Note: "readysave" is not yet in the
  design-system-next package as of this spec. // Until added: use
  spr-logo name="finances" theme="white" with a custom // ReadySave
  wordmark next to it, or request the logo asset from design.
```

### 10. Open Questions

These items require a decision before or during the engineering planning
phase.

  -------- --------------------------------- -------------- ----------------------
  **\#**   **Question**                      **Owner**      **Impact if
                                                            Unresolved**

  1        ReadySave is not yet a live       Product        Screen 8 CTA
           Sprout Finances product. Is it                   destinations change
           available for the demo build, or                 
           should Screen 8 (Stable/Strong)                  
           route to a waitlist / "coming                   
           soon" state?                                    

  2        GCash API partnership is          Product /      Screen 3 consent copy
           post-hackathon. In the production Partnerships   and data handling
           build, does Tier 1 connect via                   change
           Open Finance API or a                            
           Sprout-GCash bilateral agreement?                

  3        What is the pre-approved          Product        Screen 8 ReadyCash
           ReadyCash limit for the demo                     before/after numbers
           persona? The projection shows                    
           ₱10,000 --- is this a valid demo                 
           limit?                                           

  4        ReadyWage earned balance          Engineering    Screen 8 ReadyWage
           increases daily. Should Screen 8                 projection accuracy
           show the actual current earned                   
           balance, or a fixed demo value?                  

  5        The archetype system (Section 6,  Product        Screen 7 diagnosis
           Screen 7) uses 4 archetypes. Is                  accuracy and tone
           the classification algorithm                     
           reviewed by the product team, or                 
           should engineering propose one                   
           for sign-off?                                    

  6        Runway analysis date: should the  Product        daysToDate() reference
           calendar date in the stacked                     date
           result reflect the actual                        
           analysis date, or always                         
           reference the most recent payslip                
           date?                                            
  -------- --------------------------------- -------------- ----------------------

### 11. Agent Specifications

Runway uses four agents. Two are processing agents that run once during
data ingestion. Two are generative agents that run once after insight
extraction. No agent computes a runway number, calculates a scenario
delta, makes a product recommendation, or determines a zone. Those are
deterministic functions. Agents generate language and structured data
--- the computation layer handles math and routing.

| **Agent boundary rule**                                               |
|                                                                       |
| If the task can be expressed as a pure function with deterministic    |
| output for the same inputs, it must be a pure function --- not an     |
| agent. Agents are only used where the task requires reasoning across  |
| multiple signals simultaneously, or generating contextual natural     |
| language that references the user's specific data.                   |

### 11.0 Data Flow Overview

```
User uploads CSV / connects GCash ↓ Stage 1--3: Format detection +
  rule-based categorisation ↓ [Agent 1] Categorisation Fallback Input:
  UnresolvedTransaction[] Output: CategorisedTransaction[]
  (category + confidence per row) ↓ Stage 5: Aggregation →
  CategoryBreakdown ↓ [Agent 2] Insight Extraction Input:
  CategoryBreakdown + RunwayState Output: InsightProfile (archetype,
  dangers, trends, top merchants) ↓
  ├─────────────────────────────────────────────────────┐ ↓ ↓ [Agent 3]
  Scenario Generation [Agent 4] Diagnosis Narrative Input:
  InsightProfile + RunwayState Input: InsightProfile + RunwayState
  Output: Scenario[] Output: DiagnosisContent ↓ ↓ Screen 6: Scenario
  Playground Screen 7: Diagnosis (deltas computed deterministically)
  (copy rendered from DiagnosisContent) ↓ Zone → Product routing
  (deterministic) ↓ Screen 8: Action Card
```

Agents 3 and 4 receive the same InsightProfile and run in parallel after
Agent 2 completes. Neither depends on the other's output.

### 11.1 Agent 1 --- Categorisation Fallback

| **Model**                         | **Temperature**                   |
|                                   |                                   |
| gpt-4o-mini                       | 0 --- deterministic               |
|                                   |                                   |
| **When it runs**                  | **Timeout**                       |
|                                   |                                   |
| Stage 4 of the ingestion          | 10 seconds. On timeout: classify  |
| pipeline. After rule-based        | all timed-out rows as misc, flag  |
| lookup. Only receives unresolved  | for correction.                   |
| rows.                             |                                   |
|                                   | **Retry**                         |
| **Trigger**                       |                                   |
|                                   | 1 retry on failure. Never retry   |
| unresolvedTransactions.length >  | indefinitely.                     |
| 0                                 |                                   |
|                                   | **Cost profile**                  |
| **Batch size**                    |                                   |
|                                   | \~50 unresolved rows × 30 tokens  |
| 50 transactions per API call.     | each = \~1,500 input tokens per   |
| Parallelise if > 50.             | user. Negligible.                 |

**Input Schema**

```
type Agent1Input = { transactions: Array<{ id: string // stable row ID
  for response matching normDesc: string // cleaned description string
  (uppercase, stripped) amount: number // always positive outflow source:
  string // which bank format }> categories: CategoryKey[] // the
  allowed category list }
```

**Output Schema**

```
type Agent1Output = Array<{ id: string // matches input id category:
  CategoryKey merchant: string // normalised merchant name, best guess
  confidence: number // 0.0 -- 1.0 }>
```

**System Prompt**

```
You are a financial transaction categoriser for users in the
  Philippines. Given a list of bank/e-wallet transaction descriptions,
  assign each to exactly one category. Return only a valid JSON array ---
  no preamble, no explanation. Categories: food_dining restaurants, food
  delivery, coffee shops groceries supermarkets, wet market, convenience
  stores, grocery delivery bills_utilities electricity, water, internet,
  phone load, streaming bills transport ride-hailing, fuel, transit card
  top-up, toll RFID shopping retail, online marketplace, clothing,
  electronics, hardware health_wellness pharmacy, clinic, hospital, HMO
  premium, gym housing rent, condo dues, HOA dues, property management
  fees transfers money transfers, remittances, padala, GCash Send Money
  entertainment_subs streaming services, gaming, cinema, events,
  activities misc cannot determine --- use only when no other category
  fits Philippine-specific rules: - Grab Food / foodpanda / delivery →
  food_dining - Angkas / JoyRide / Beep / EasyTrip → transport - Mercury
  Drug / Watsons / Generika → health_wellness - Palawan Express / LBC /
  Western Union → transfers - Bayad Center / ECPay → bills_utilities -
  Description contains a person's name + amount → transfers - Recurring
  transfer > ₱5,000 to same name/account → likely housing (rent) - ATM /
  WITHDRAWAL → misc (cash, cannot classify further) - GCASH QR with no
  merchant detail → misc Confidence scoring: 1.0 exact known merchant 0.8
  strong keyword match 0.6 plausible but uncertain 0.4 best guess, low
  signal Set confidence < 0.7 for anything you are not confident about.
  Never return confidence = 0 --- always assign a best guess.
```

**User Prompt Template**

```
Categorise these transactions. Return JSON array only: [
  {"id":"tx_001","category":"food_dining","merchant":"Grab
  Food","confidence":0.95}, \... ] Transactions: ${batch.map(t =>
  `${t.id}: "${t.normDesc}" ₱${t.amount}
  (${t.source})`).join("\\n")}
```

**Failure Modes & Handling**

  --------------------- -------------------------------------------------
  **Failure**           **Handling**

  API timeout (> 10s)  All rows in batch → category: misc, confidence:
                        0, reason: "timeout". Flagged in correction UI.

  Malformed JSON        1 retry. If still malformed: same as timeout
  response              handling.

  Missing id in         Match by array position if count matches.
  response              Otherwise: misc fallback.

  Confidence = 1.0 for  Accept --- do not distrust high-confidence
  all rows              outputs.

  Unknown category      Map to misc. Log the unknown value for seed list
  string returned       improvement.
  --------------------- -------------------------------------------------

### 11.2 Agent 2 --- Insight Extraction

Agent 2 runs once after category aggregation. It reasons over the full
CategoryBreakdown and RunwayState to produce an InsightProfile --- a
structured object that feeds both Agent 3 (scenario generation) and
Agent 4 (diagnosis narrative). It also produces the danger signals shown
on Screen 5.

| **Model**                         | **Timeout**                       |
|                                   |                                   |
| gpt-4o                            | 15 seconds.                       |
|                                   |                                   |
| **When it runs**                  | **Output format**                 |
|                                   |                                   |
| Once, after Stage 5 aggregation.  | Strict JSON. Prompt instructs:    |
| Blocking --- Agents 3 and 4 wait  | return only valid JSON, no        |
| for its output.                   | preamble.                         |
|                                   |                                   |
| **Temperature**                   | **Cost profile**                  |
|                                   |                                   |
| 0.2 --- slight variation for      | \~800 input tokens (aggregated    |
| natural language fields,          | data, not raw transactions).      |
| deterministic for structured      | \~400 output tokens.              |
| fields                            |                                   |

**Input Schema**

```
type Agent2Input = { state: RunwayState // liquidCash, monthlyBurn,
  takeHome, categories baselineDays: number // computed before agent call
  zone: ZoneName monthlyAmounts: { // per category, per month --- for
  trend analysis [category: CategoryKey]: number[] // index 0 =
  oldest month } topMerchants: { // top 3 merchants per category by total
  spend [category: CategoryKey]: Array<{ name: string, monthlyAvg:
  number }> } monthCount: number // how many months of data (3 or 4)
  analysisDate: string // ISO date string }
```

**Output Schema --- InsightProfile**

```
type InsightProfile = { archetype: { key: "lifestyle_inflator" \|
  "steady_spender" \| "resilient_saver" \| "crisis_mode" name:
  string // display name, e.g. "The Lifestyle Inflator" signal: string
  // one sentence explaining classification basis } dangerSignals:
  Array<{ severity: "danger" \| "caution" title: string // short
  label for Screen 5 card detail: string // one sentence with specific
  numbers metric: string // badge text, e.g. "+38%" or "₱23,900 gap"
  category?: CategoryKey // which category triggered this }> trends:
  Array<{ category: CategoryKey direction: "growing" \| "stable" \|
  "declining" pctChange: number // month-over-month % change (last 2
  months) notable: boolean // true if worth surfacing in diagnosis
  topMerchant: string // highest-spend merchant in this category
  topMerchantAmount: number // their monthly average }> remittanceNote:
  string \| null // null if remittances < 15% of burn // populated if
  >= 15%: e.g. // "₱5,000 of your monthly outflow goes to family ---
  that's a commitment, not a cut." flexibleBurn: number // sum of
  discretionary category averages only fixedBurn: number // sum of
  essential + committed category averages }
```

**System Prompt**

```
You are a financial analyst building a resilience profile for a Sprout
  payroll user in the Philippines. Given their categorised spending data
  and runway state, produce a structured InsightProfile as valid JSON
  only --- no preamble. Archetype classification rules:
  lifestyle_inflator burn > takeHome AND at least one discretionary
  category has grown > 15% over the analysis period steady_spender burn
  within 20% of takeHome, no strong trend in any category resilient_saver
  burn < takeHome consistently across all months crisis_mode burn > 2x
  takeHome OR baselineDays < 30 Danger signal rules --- generate a
  signal when: - Any discretionary category grew > 25% month-over-month
  (severity: danger) - Any discretionary category has grown > 10% for 3
  consecutive months (severity: caution) - monthlyBurn > takeHome
  (severity: danger --- always include if true) - A single merchant
  accounts for > 20% of total burn (severity: caution) - Two or more
  discretionary categories are growing simultaneously (severity: caution)
  Maximum 3 danger signals. Prioritise by severity then by impact on
  burn. Detail sentences must use the user's actual numbers. Never
  invent numbers. Only reference values present in the input.
  remittanceNote: populate only if transfers category monthlyAverage >=
  (monthlyBurn * 0.15). Use this exact template: "₱{amount} of your
  monthly outflow goes to family --- that's a commitment, not a cut."
  flexibleBurn = sum of: food_dining + shopping + entertainment_subs +
  misc fixedBurn = sum of: groceries + bills_utilities + transport +
  health_wellness + housing + transfers
```

**Archetype Definitions**

  -------------------- ---------------- --------------------- -----------------
  **Key**              **Display Name** **Core Signal**       **Tone in
                                                              Diagnosis**

  lifestyle_inflator   The Lifestyle    Burn > income and    Direct but not
                       Inflator         growing. Spending has judgmental. Names
                                        expanded to fill (and the pattern
                                        exceed) the salary.   without shaming
                                                              it.

  steady_spender       The Steady       Burn roughly matches  Encouraging. The
                       Spender          income. Stable but no habit is good ---
                                        buffer growth.        the missing piece
                                                              is the buffer.

  resilient_saver      The Resilient    Burn consistently     Affirming. Focus
                       Saver            below income. Buffer  on making the
                                        is growing.           buffer work
                                                              harder.

  crisis_mode          On the Edge      Burn is 2x+ income,   Calm and
                                        or runway < 30 days. specific. No
                                        Urgent.               alarm language.
                                                              One clear next
                                                              action.
  -------------------- ---------------- --------------------- -----------------

**Failure Modes & Handling**

  --------------------- -------------------------------------------------
  **Failure**           **Handling**

  API timeout           Generate InsightProfile from deterministic
                        fallback: archetype determined by burn vs
                        takeHome ratio only, no danger signals, no
                        trends, remittanceNote null. Screen 7 shows a
                        simplified diagnosis.

  Archetype key not in  Default to steady_spender. Log the invalid value.
  allowed list          

  dangerSignals array   Valid --- not every user has danger signals.
  empty                 Screen 5 shows "No major signals detected."

  flexibleBurn >       Data error. Log and recompute deterministically
  monthlyBurn           from category breakdown.
  --------------------- -------------------------------------------------

### 11.3 Agent 3 --- Scenario Generation

Agent 3 takes the InsightProfile and RunwayState and generates a
personalised set of Scenario objects for Screen 6. It does not compute
deltas --- it produces typed Scenario objects that the computation model
processes deterministically. The agent's job is to decide which
scenarios are relevant for this user and what their specific parameters
should be.

| **Model**                         | **Output format**                 |
|                                   |                                   |
| gpt-4o-mini                       | Strict JSON array of Scenario     |
|                                   | objects. No prose.                |
| **When it runs**                  |                                   |
|                                   | **Max scenarios**                 |
| After Agent 2. In parallel with   |                                   |
| Agent 4.                          | 5 predefined + 1 custom slot.     |
|                                   | Agent returns 3--5 predefined     |
| **Temperature**                   | scenarios.                        |
|                                   |                                   |
| 0 --- scenario objects must be    | **Cost profile**                  |
| deterministic and structurally    |                                   |
| valid                             | \~500 input tokens. \~300 output  |
|                                   | tokens.                           |

**Input Schema**

```
type Agent3Input = { insightProfile: InsightProfile state: RunwayState
  baselineDays: number }
```

**Output Schema**

```
// Agent 3 returns an array of Scenario objects (see Section 4.3.2 for
  full type) // The agent populates: id, type, label, effort, recurrence,
  params, assumption // The computation layer populates: delta (computed
  after agent returns) type Agent3Output = Scenario[] // Example
  output: [ { "id": "sc_grab_baseline", "type": "SPENDING_CUT",
  "label": "Return Grab Food to July levels", "effort": "habit",
  "recurrence": "recurring", "params": { "category":
  "food_dining", "cutAmount": 4200 }, "assumption": null }, {
  "id": "sc_dining_cut", "type": "SPENDING_CUT", "label": "Cut
  dining & delivery 70%", "effort": "habit", "recurrence":
  "recurring", "params": { "category": "food_dining", "cutPct":
  0.70 }, "assumption": null }, { "id": "sc_salary_raise",
  "type": "INCOME_GAIN", "label": "Salary raise 10%", "effort":
  "life", "recurrence": "recurring", "params": { "gainAmount":
  2850 }, "assumption": "Assumes raise takes effect immediately" } ]
```

**System Prompt**

```
You are building personalised financial scenarios for a Runway user in
  the Philippines. Given their spending profile and runway state,
  generate 3--5 Scenario objects as a valid JSON array. Return JSON only
  --- no preamble, no explanation. Rules for scenario selection: 1. Only
  generate SPENDING_CUT scenarios for discretionary categories
  (food_dining, shopping, entertainment_subs). Never for essential or
  committed. 2. If a specific merchant accounts for > 20% of a
  category's spend, generate a merchant-specific scenario first
  ("Return [merchant] to [month] levels") before a generic category
  cut. 3. cutAmount for merchant scenarios = currentMonthAmount -
  baselineMonthAmount where baseline = average of months excluding the
  most recent. 4. Always include one INCOME_GAIN scenario (salary raise
  or side hustle). 5. Include HOUSING_CHANGE only if user has housing
  spend in their data. Default rentDelta: +15000. Set assumption field.
  6. Never generate a scenario for transfers or remittances category. 7.
  Never generate a scenario that would result in negative burn. 8. Effort
  tags: quick = cancellable subscription / one-time action, habit =
  sustained behaviour change, life = structural decision. 9. Label must
  be specific and personal. Use actual merchant names and actual amounts
  from the data. Never use generic labels like "Cut spending". 10.
  Maximum one HOUSING_CHANGE scenario. Maximum two SPENDING_CUT
  scenarios. Allowed category keys: food_dining, groceries,
  bills_utilities, transport, shopping, health_wellness, housing,
  transfers, entertainment_subs, misc Allowed type values: SPENDING_CUT,
  INCOME_GAIN, ONE_TIME_INJECT, HOUSING_CHANGE, CUSTOM Allowed effort
  values: quick, habit, life Allowed recurrence values: one-time,
  recurring
```

**Post-Processing (After Agent Returns)**

```
// After Agent 3 returns, the computation layer: // 1. Validates each
  Scenario object against the type schema // 2. Drops any scenario where
  computeDelta(scenario, state) === 0 // 3. Computes delta for each
  scenario deterministically // 4. Sorts by: effort asc (quick first),
  then delta desc (highest impact first) // 5. Appends the fixed "+
  Custom scenario" chip last // 6. Identifies fastestWin = first
  scenario where effort !== "life" function validateScenario(s:
  unknown): s is Scenario { // Check required fields, allowed
  type/effort/recurrence values // Check params shape matches type //
  Return false if invalid --- invalid scenarios are dropped, not errored
  }
```

**Failure Modes & Handling**

  --------------------- -------------------------------------------------
  **Failure**           **Handling**

  Agent returns < 3    Supplement with hardcoded fallback scenarios
  valid scenarios       using actual state values: generic dining cut
                        (70% of food_dining avg), salary raise (10% of
                        takeHome), side hustle (₱10,000 INCOME_GAIN).

  Agent returns invalid Drop the invalid object. Log it. Do not show
  Scenario object       error to user.

  cutAmount > category Cap cutAmount at categoryMonthlyAverage. Log the
  monthlyAverage        capped value.

  HOUSING_CHANGE with   Drop the scenario. Do not generate housing
  no housing data       scenarios if housing monthlyAverage === 0.
  --------------------- -------------------------------------------------

### 11.4 Agent 4 --- Diagnosis Narrative

Agent 4 generates the copy for Screen 7. It receives the InsightProfile
and RunwayState and returns three structured text fields: what is
happening, what to do about it, and the honest take. It must reference
the user's actual numbers and merchant names --- not generic financial
advice.

| **Model**                         | **Timeout**                       |
|                                   |                                   |
| gpt-4o                            | 15 seconds.                       |
|                                   |                                   |
| **When it runs**                  | **Output format**                 |
|                                   |                                   |
| After Agent 2. In parallel with   | Strict JSON --- three string      |
| Agent 3.                          | fields with hard character        |
|                                   | limits.                           |
| **Temperature**                   |                                   |
|                                   | **Cost profile**                  |
| 0.4 --- natural variation in      |                                   |
| phrasing while staying factually  | \~700 input tokens. \~350 output  |
| grounded                          | tokens.                           |

**Input Schema**

```
type Agent4Input = { insightProfile: InsightProfile state: RunwayState
  baselineDays: number zone: ZoneName fastestWin: { // from Agent 3
  post-processing label: string delta: number // days added newDays:
  number // baseline + delta } }
```

**Output Schema --- DiagnosisContent**

```
type DiagnosisContent = { archetypeName: string // display name, from
  InsightProfile.archetype.name whatIsHappening: string // 2--4
  sentences. Must include: // - monthly burn amount // - take-home pay
  amount // - monthly gap (if burn > income) // - baseline runway days
  // - top growing merchant by name and amount // Max 280 characters
  whatToDoAboutIt: string // 2--3 sentences. Must reference the
  fastestWin scenario by label. // Must include the before/after runway
  numbers. // Frame as a concrete action, not general advice. // Max 240
  characters honestTake: string // 1--2 sentences. Italic in UI. Plain
  language summary. // Must not repeat numbers already stated above. //
  Must not use alarm language or shame language. // Captures the
  underlying pattern, not just the symptom. // Max 180 characters }
```

**System Prompt**

```
You are writing a personal financial resilience report for a Sprout
  payroll user in the Philippines. Your tone is direct, honest, and
  grounded --- not alarming, not encouraging, not salesy. You write like
  a trusted friend who happens to know finance, not like a financial
  advisor covering liability. Generate a DiagnosisContent object as valid
  JSON only. No preamble. Rules for whatIsHappening: - Use the user's
  actual numbers. Every sentence must contain a specific figure. - Name
  the top growing merchant explicitly (e.g. "Grab Food alone is
  ₱10,200"). - State the monthly gap in plain terms if burn >
  takeHome. - Do not editorialize. State facts. - 2--4 sentences. Max 280
  characters. Rules for whatToDoAboutIt: - Reference the fastestWin
  scenario by its exact label. - State the before and after runway days
  explicitly. - Frame as one specific change, not a list. - 2--3
  sentences. Max 240 characters. Rules for honestTake: - Do not repeat
  numbers from the sections above. - Name the underlying pattern (e.g.
  lifestyle inflation, income-spend gap). - No alarm language: never use
  "crisis", "danger", "urgent", "warning". - No shame language:
  never use "overspending", "irresponsible", "reckless". -
  Italicised in UI --- write in a reflective, not instructional,
  register. - 1--2 sentences. Max 180 characters. If remittanceNote is
  present in InsightProfile: - Include it verbatim in whatIsHappening as
  a standalone sentence. - Never suggest cutting remittances in
  whatToDoAboutIt. Character limits are hard limits. If you exceed them,
  truncate cleanly at a sentence boundary --- never mid-sentence.
```

**Example Output --- Alex Garcia (Lifestyle Inflator, Stable Zone)**

| **whatIsHappening:**                                                  |
|                                                                       |
| *"Your monthly burn has grown to ₱52,400 --- but your take-home is   |
| ₱28,500. Your savings are covering a ₱23,900 monthly gap. Grab Food   |
| alone is ₱10,200 this month, up 38% from three months ago."*         |
|                                                                       |
| **whatToDoAboutIt:**                                                  |
|                                                                       |
| *"Return Grab Food to July levels --- that single change cuts your   |
| monthly burn by ₱4,200. Your runway goes from 103 days to 122         |
| days."*                                                              |
|                                                                       |
| **honestTake:**                                                       |
|                                                                       |
| *"103 days is not a crisis --- but the gap between what comes in and |
| what goes out has been quietly widening for three months."*          |

**Failure Modes & Handling**

  --------------------- -------------------------------------------------
  **Failure**           **Handling**

  API timeout           Show deterministic fallback copy built from
                        InsightProfile fields and state values using
                        template strings. Fallback must be pre-built and
                        available without any LLM call.

  Character limit       Truncate at last sentence boundary before the
  exceeded              limit. Never show a mid-sentence truncation to
                        the user.

  Response contains     Post-processing filter: check for banned words
  shame or alarm        ("crisis", "danger", "urgent", "warning",
  language              "overspending", "irresponsible",
                        "reckless"). If found: use fallback template
                        for that field only.

  Numbers in response   Post-processing validation: verify that monetary
  do not match state    figures in the copy exist in the input data ±5%.
                        If mismatch found: replace with fallback
                        template.

  honestTake repeats    Accept --- the character limit naturally
  numbers from above    constrains this. Do not add a post-processing
                        check for it in v1.
  --------------------- -------------------------------------------------

### 11.5 Deterministic Fallback Templates

All four agents have deterministic fallbacks. The fallback system must
be implemented before the agents --- it is the guaranteed baseline, not
an afterthought. Agents are the upgrade.

```
// Agent 4 fallback --- built from state values, no LLM required
  function buildFallbackDiagnosis(state: RunwayState, profile:
  InsightProfile): DiagnosisContent { const gap = state.monthlyBurn -
  state.takeHome const hasGap = gap > 0 return { archetypeName:
  profile.archetype.name, whatIsHappening: hasGap ? `Your monthly burn
  is ₱${fmt(state.monthlyBurn)}, but your take-home is
  ₱${fmt(state.takeHome)}. ` + `Your savings are covering a
  ₱${fmt(gap)} monthly gap.` : `Your monthly burn is
  ₱${fmt(state.monthlyBurn)} against a take-home of
  ₱${fmt(state.takeHome)}.`, whatToDoAboutIt: `Your highest-impact
  move is to reduce your discretionary spend. ` + `Even a ₱5,000
  monthly reduction adds ${Math.round(5000 / (state.monthlyBurn / 30))}
  days to your runway.`, honestTake: `The number that matters most is
  the gap between what comes in and what goes out.` } }
```

| **Implementation order**                                              |
|                                                                       |
| -   Build deterministic fallback for all four agents first            |
|                                                                       |
| -   Write unit tests against fallback outputs                         |
|                                                                       |
| -   Implement agent calls as an enhancement layer on top              |
|                                                                       |
| -   All tests must pass with fallback active before agent integration |
|     begins                                                            |
|                                                                       |
| -   This is true TDD for the agent layer --- the fallback is the      |
|     red/green baseline                                                |

### 11.6 Agent Test Cases

Required integration tests for the agent layer. Each test must run
against a fixed input and assert on the output schema --- not the
specific generated values, since LLM outputs vary.

  ----------- ---------------------------- ---------------------------------
  **Agent**   **Test**                     **Assert**

  Agent 1     Input: "GRAB*FOOD          category === "food_dining",
              PHILIPPINES 12345"          confidence >= 0.8

  Agent 1     Input: "GCASH QR            category === "misc", confidence
              09171234567"                <= 0.5

  Agent 1     Input: "MERCURY DRUG BGC    category === "health_wellness"
              BRANCH"                     

  Agent 1     Input: "JUAN DELA CRUZ      category === "transfers"
              ₱5000"                      

  Agent 1     API timeout simulation       All rows return misc, flagged:
                                           true. No exception thrown.

  Agent 2     Alex Garcia dataset (food    archetype.key ===
              growing 38%, burn >         "lifestyle_inflator",
              takeHome)                    dangerSignals.length >= 2

  Agent 2     Dataset where burn <        archetype.key ===
              takeHome for all months      "resilient_saver",
                                           dangerSignals.length === 0

  Agent 2     transfers category = 18% of  remittanceNote !== null,
              burn                         remittanceNote contains "₱"

  Agent 2     API timeout simulation       Returns valid InsightProfile from
                                           fallback. archetypeName is set.

  Agent 3     Alex Garcia InsightProfile   At least one scenario label
              (Grab Food 38% spike)        contains "Grab Food".
                                           SPENDING_CUT present.

  Agent 3     InsightProfile with no       No SPENDING_CUT scenarios.
              discretionary spend          INCOME_GAIN present.

  Agent 3     Any valid input              All returned objects pass
                                           validateScenario(). Array length
                                           3--5.

  Agent 4     Alex Garcia InsightProfile   All three fields present. Each
                                           within character limit. No banned
                                           words.

  Agent 4     remittanceNote present in    whatIsHappening contains
              InsightProfile               remittanceNote text.
                                           whatToDoAboutIt does not suggest
                                           cutting remittances.

  Agent 4     API timeout simulation       Returns fallback
                                           DiagnosisContent. No exception.
                                           Numbers in copy match state.
  ----------- ---------------------------- ---------------------------------

### 12. Demo Mode --- Full Specification

This section is the complete implementation reference for
DEMO_MODE=true. It covers the agent stub/live decision, Screen 4 AI
Tower visibility, Screen 7 attribution, all fixture data for Alex
Garcia, and the demo run checklist.

| **AI Tower positioning for the demo**                                 |
|                                                                       |
| Runway qualifies as an AI Tower product on one condition: the         |
| diagnosis copy on Screen 7 must be real GPT output derived from       |
| Alex's actual transaction data --- not a template with variables     |
| substituted in. That is the moment a judge sees reasoning, not        |
| formatting. Everything else in the demo can be pre-computed. That one |
| screen must be live.                                                  |

### 12.1 Agent Stub / Live Decision

  ---------------- ---------- --------------------------- -------------------------------------------
  **Agent**        **Demo     **Reason**                  **Fallback if live call fails**
                   Mode**                                 

  Agent 1 ---      STUBBED    Demo uses a pre-cleaned     N/A --- fixture always available
  Categorisation              dataset. No messy CSV to    
                              resolve.                    

  Agent 2 ---      STUBBED    InsightProfile is           N/A --- fixture always available
  Insight                     deterministic for one       
  Extraction                  persona. Pre-computing it   
                              removes all latency from    
                              the Screen 4--5 transition. 

  Agent 3 ---      STUBBED    Scenario chips are visible  N/A --- fixture always available
  Scenario                    in the prototype.           
  Generation                  Pre-computing them ensures  
                              chip labels match the       
                              diagnosis copy exactly.     

  Agent 4 ---      LIVE       This is the AI Tower        Pre-written fallback in
  Diagnosis                   moment. The diagnosis copy  /src/demo/fixtures/fallback-diagnosis.ts.
  Narrative                   must be real GPT output to  Activates automatically on timeout > 10s.
                              land with judges.           
  ---------------- ---------- --------------------------- -------------------------------------------

### 12.2 DEMO_MODE Routing Pattern

Each agent function checks DEMO_MODE at the top before making any API
call. The fixture import is a static file --- no async, no network, no
failure mode.

```
// Pattern used by Agents 1, 2, and 3 import { DEMO_MODE } from
  "@/config/build" import { alexGarciaInsightProfile } from
  "@/demo/fixtures/insight-profile" async function
  runInsightExtraction(input: Agent2Input): Promise<InsightProfile> {
  if (DEMO_MODE) { // Simulate processing delay so Screen 4 animation has
  time to complete await delay(DEMO_PROCESSING_DELAY_MS) return
  alexGarciaInsightProfile } return callOpenAI(input) // production path
  } // Agent 4 --- live in both modes, with timeout fallback async
  function runDiagnosisNarrative(input: Agent4Input):
  Promise<DiagnosisContent> { try { const result = await
  Promise.race([ callOpenAI(input), timeout(10_000) ]) return result }
  catch { return DEMO_MODE ? alexGarciaFallbackDiagnosis :
  buildFallbackDiagnosis(input) } } // Build config export const
  DEMO_MODE = process.env.NEXT_PUBLIC_DEMO_MODE === "true" export const
  DEMO_PROCESSING_DELAY_MS = 3200 // matches Screen 4 animation duration
```

### 12.3 Screen 4 --- AI Tower Visibility

Screen 4 is the transformation moment. The judge needs to see raw data
go in and structured insight come out. The animation must make the AI's
work visible --- not just a spinner.

**Animation Sequence (3.2 seconds total)**

  ---------- ------------------------------------ ---------------------------
  **Time**   **What the user sees**               **Implementation note**

  0.0s       Header: "Reading your               Immediate on screen load
             transactions\..." Progress bar at   
             0%.                                  

  0.2s       Raw transaction strings begin        Each string appears in a
             appearing one by one, fading in at   monospace font,
             180ms intervals. See list below.     left-aligned, slightly
                                                  muted. 6 strings total.

  1.3s       Progress bar fills to 40%. Header    CSS transition on progress
             changes to "Categorising            bar
             transactions\..."                   

  1.6s       Raw strings fade out. Category rows  Staggered fade-in, 200ms
             begin appearing: food icon + "Food  per row
             & Dining" + amount badge.           

  2.4s       Progress bar fills to 100%. All 6    
             category rows visible.               

  2.8s       Header: "Analysis complete."       
             Checkmark animation.                 

  3.2s       Auto-advance to Screen 5.            800ms pause after complete
  ---------- ------------------------------------ ---------------------------

**Raw Transaction Strings --- Demo Sequence**

These six strings are shown during the 0.2s--1.3s window. They are
chosen to represent the range of Alex's data --- recognisable
merchants, a peer transfer, a utility. They make the AI's
categorisation job visible.

```
const DEMO_TRANSACTION_STRINGS = [ "GRAB*FOOD PHILIPPINES 09:42
  ₱340.00", "GCASH SEND MONEY NENA G. ₱5,000.00", "SM SUPERMARKET MOA
  14:21 ₱2,840.00", "MERALCO PAYMENT ECPay ₱3,120.00", "LAZADA
  PAYMENTS PTE LTD ₱1,299.00", "SHOPEE PAY SPAY-241103 ₱890.00", ]
```

Strings are shown in monospace (font-family: "Courier New"). Color:
MUSHROOM-400. Font size: 13px. Each fades in over 120ms. No hover state,
no interactivity.

**Category Row Appearance Order**

Categories appear in impact order --- highest monthly average first.
This order matches the burn breakdown on Screen 5 and reinforces the
narrative direction.

```
const DEMO_CATEGORY_ORDER = [ { key: "food_dining", label: "Food &
  Dining", amount: "₱14,200" }, { key: "groceries", label:
  "Groceries & Market", amount: "₱8,000" }, { key:
  "bills_utilities", label: "Bills & Utilities", amount: "₱7,200"
  }, { key: "transport", label: "Transport", amount: "₱6,500" }, {
  key: "shopping", label: "Shopping", amount: "₱5,000" }, { key:
  "transfers", label: "Transfers & Family", amount: "₱5,000" }, {
  key: "entertainment_subs", label: "Entertainment & Subs", amount:
  "₱3,300" }, { key: "health_wellness", label: "Health & Wellness",
  amount: "₱2,800" }, { key: "misc", label: "Miscellaneous",
  amount: "₱400" }, ] // Total: ₱52,400 --- matches stated monthlyBurn
  // Reconciliation note: earlier spec sections show illustrative
  per-category // amounts that sum to ₱62,000. These fixture values are
  the authoritative // numbers for demo and for unit tests. The ₱52,400
  monthlyBurn is fixed.
```

### 12.4 Screen 7 --- AI Attribution

Screen 7 should make the AI's contribution visible without being loud
about it. Two elements:

**Attribution Line**

A single small line below the diagnosis card body:

```
Generated from 247 transactions · Sprout AI
```

Implementation: spr-body-xs, color MUSHROOM-400, centered, italic. The
transaction count (247) is a static string in demo mode. In production
it reflects the actual transaction count from Agent 1.

**Processing State on Screen 7**

Screen 7 has a brief loading state while Agent 4 runs. This is visible
--- not hidden behind a full-screen loader. The archetype name and
pattern card appear immediately from the InsightProfile fixture. The
three diagnosis blocks (whatIsHappening, whatToDoAboutIt, honestTake)
show a subtle shimmer while the live call resolves.

```
// Screen 7 render order 1. Archetype card renders immediately (from
  Agent 2 fixture --- instant) 2. Diagnosis blocks render as skeleton
  shimmer 3. Agent 4 call fires (live gpt-4o) 4. On response: shimmer
  fades, copy fades in over 400ms 5. Attribution line appears after copy
  is fully visible // If judge is watching: they see the AI "thinking"
  for \~2s, then // the specific diagnosis copy appearing. That's the
  Tower moment.
```

### 12.5 Fixture Data --- Alex Garcia

The following objects are committed to /src/demo/fixtures/ and imported
by the DEMO_MODE routing in Section 12.2. These are the canonical demo
values. All unit tests that reference Alex Garcia's data use these
objects.

### 12.5.1 RunwayState (alex-garcia-state.ts)

```
export const alexGarciaState: RunwayState = { liquidCash: 180_000,
  monthlyBurn: 52_400, takeHome: 28_500, categories: { food_dining:
  14_200, groceries: 8_000, bills_utilities: 7_200, transport: 6_500,
  shopping: 5_000, transfers: 5_000, entertainment_subs: 3_300,
  health_wellness: 2_800, housing: 0, misc: 400, }, } // Derived
  (computed, not stored) // baselineDays = Math.floor(180000 / (52400 /
  30)) = 103 // zone = "stable" // dailyBurn = 52400 / 30 = 1746.67
```

### 12.5.2 InsightProfile (insight-profile.ts) --- Agent 2 fixture

```
export const alexGarciaInsightProfile: InsightProfile = { archetype: {
  key: "lifestyle_inflator", name: "The Lifestyle Inflator", signal:
  "Monthly burn exceeds take-home by ₱23,900 and food & dining has grown
  38% over 3 months.", }, dangerSignals: [ { severity: "danger",
  title: "Monthly spend exceeds take-home pay", detail: "Your ₱52,400
  monthly burn is ₱23,900 above your ₱28,500 take-home. Your savings are
  covering the gap.", metric: "₱23,900 gap", category: undefined, }, {
  severity: "caution", title: "Food & dining growing fast", detail:
  "Grab Food alone is ₱10,200 this month --- up 38% from your 3-month
  average of ₱7,390.", metric: "+38%", category: "food_dining", },
  ], trends: [ { category: "food_dining", direction: "growing",
  pctChange: 38, notable: true, topMerchant: "Grab Food",
  topMerchantAmount: 10_200, }, { category: "shopping", direction:
  "stable", pctChange: 4, notable: false, topMerchant: "Lazada",
  topMerchantAmount: 2_800, }, { category: "entertainment_subs",
  direction: "stable", pctChange: 0, notable: false, topMerchant:
  "Netflix", topMerchantAmount: 899, }, { category: "transfers",
  direction: "stable", pctChange: 0, notable: false, topMerchant:
  "GCash Send Money", topMerchantAmount: 5_000, }, ], remittanceNote:
  null, // transfers = ₱5,000 / ₱52,400 = 9.5% --- below 15% threshold
  flexibleBurn: 22_900, // food_dining(14200) + shopping(5000) +
  entertainment_subs(3300) + misc(400) fixedBurn: 29_500, //
  groceries(8000) + bills(7200) + transport(6500) + health(2800) +
  housing(0) + transfers(5000) }
```

### 12.5.3 Scenarios (scenarios.ts) --- Agent 3 fixture

```
export const alexGarciaScenarios: Scenario[] = [ { id:
  "sc_grab_baseline", type: "SPENDING_CUT", label: "Return Grab Food
  to July levels", effort: "habit", recurrence: "recurring", params:
  { category: "food_dining", cutAmount: 2_810 }, assumption: null, //
  cutAmount = 10200 (Sep) - 7390 (3-month avg) = 2810 // delta =
  Math.floor(180000 / ((52400 - 2810) / 30)) - 103 = +5 days // Note:
  displayed as fastestWin --- highest delta among habit-tier }, { id:
  "sc_dining_cut", type: "SPENDING_CUT", label: "Cut dining &
  delivery 70%", effort: "habit", recurrence: "recurring", params: {
  category: "food_dining", cutPct: 0.70 }, assumption: null, //
  cutAmount = 14200 * 0.70 = 9940 // delta = Math.floor(180000 /
  ((52400 - 9940) / 30)) - 103 = +20 days }, { id: "sc_salary_raise",
  type: "INCOME_GAIN", label: "Salary raise 10%", effort: "life",
  recurrence: "recurring", params: { gainAmount: 2_850 }, assumption:
  "Assumes raise takes effect immediately", // gainAmount = 28500 *
  0.10 = 2850 // newGap = (52400 - 28500) - 2850 = 21050 // effectiveBurn
  = 28500 + 21050 = 49550 // delta = Math.floor(180000 / (49550 / 30)) -
  103 = +6 days }, { id: "sc_side_hustle", type: "INCOME_GAIN",
  label: "Side hustle ₱10k/mo", effort: "habit", recurrence:
  "recurring", params: { gainAmount: 10_000 }, assumption: "Assumes
  sustained ₱10,000/month additional income", // newGap = (52400 -
  28500) - 10000 = 13900 // effectiveBurn = 28500 + 13900 = 42400 //
  delta = Math.floor(180000 / (42400 / 30)) - 103 = +24 days }, { id:
  "sc_housing", type: "HOUSING_CHANGE", label: "Move to a bigger
  unit", effort: "life", recurrence: "recurring", params: {
  rentDelta: 15_000 }, assumption: "Assumes +₱15,000/month increase in
  housing cost", // newBurn = 52400 + 15000 = 67400 // delta =
  Math.floor(180000 / (67400 / 30)) - 103 = -23 days }, ] //
  Post-processing results (computed from fixture, not stored): //
  fastestWin = sc_grab_baseline (habit tier, positive delta) // Sorted
  display order: sc_grab_baseline, sc_dining_cut, sc_side_hustle, //
  sc_salary_raise, sc_housing, [+ Custom scenario]
```

**12.5.4 Fallback Diagnosis (fallback-diagnosis.ts) --- Agent 4 timeout
fallback**

Used only if the live Agent 4 call times out during the demo.
Pre-written to match the quality bar of real GPT output for Alex's
profile.

```
export const alexGarciaFallbackDiagnosis: DiagnosisContent = {
  archetypeName: "The Lifestyle Inflator", whatIsHappening: "Your
  monthly burn has grown to ₱52,400 --- but your take-home is ₱28,500.
  " + "Your savings are covering a ₱23,900 monthly gap. " + "Grab
  Food alone is ₱10,200 this month, up 38% from three months ago.",
  whatToDoAboutIt: "Return Grab Food to July levels --- that single
  change reduces your monthly burn by ₱2,810. " + "Your runway goes
  from 103 days to 108 days. " + "Stack it with a side hustle and you
  cross 130 days.", honestTake: "103 days is not a crisis --- but the
  gap between what comes in and what goes out " + "has been quietly
  widening for three months.", }
```

### 12.6 Latency Targets --- Demo Path

Every transition the judge experiences must feel instant or intentional.
"Intentional" means the wait is visually interesting, not blank.

  ------------------ ------------ ----------------------------------------
  **Transition**     **Target**   **How achieved**

  Screen 1 → 2       < 200ms     Static data. Instant.

  Screen 2 → 3       < 200ms     Static data. Instant.

  Screen 3 → 4       < 300ms     Animation starts immediately. Data load
                                  is behind the animation.

  Screen 4 animation 3.2s total   Intentional --- judge watches AI work.
                                  DEMO_PROCESSING_DELAY_MS = 3200.

  Screen 4 → 5       Auto at 3.2s No tap required. Auto-advance after
                                  animation completes.

  Screen 5 → 6       < 200ms     All data already computed. Instant.

  Screen 6 → 7       < 300ms +   Archetype card instant (fixture).
                     2s live call Diagnosis blocks shimmer while Agent 4
                                  resolves.

  Agent 4 live call  2--4s        gpt-4o with temperature=0.4. Fallback
                     typical, 10s activates at 10s.
                     max          

  Screen 7 → 8       < 200ms     Product card pre-computed from zone.
                                  Instant.

  Scenario chip      < 50ms      Pure synchronous computation. No async.
  toggle                          
  ------------------ ------------ ----------------------------------------

### 12.7 Demo Run Checklist

To be completed before the demo session. Not optional.

  -------- ---------------------------- --------------------------------------
  **\#**   **Check**                    **How to verify**

  1        DEMO_MODE=true is set in     Console.log(DEMO_MODE) on app load ---
           build env                    must print true

  2        OpenAI API key is valid and  Run a test Agent 4 call outside the
           has quota                    demo flow. Confirm < 5s response.

  3        Agent 4 fallback activates   Set timeout to 100ms temporarily, run
           correctly                    through Screen 7. Confirm fallback
                                        copy appears.

  4        Screen 4 animation completes Run through screens 1--5 twice.
           cleanly                      Confirm animation timing feels right.

  5        Scenario chips compute       Toggle each chip. Verify deltas match
           correct deltas               comments in scenarios.ts.

  6        Screen 8 shows ReadySave     Reach Screen 8 without scenario
           (Stable zone)                toggles. Confirm ReadySave card.

  7        Screen 8 zone-switches       Toggle enough scenarios to reach
           correctly                    Fragile zone, advance to Screen 8.
                                        Confirm ReadyCash.

  8        Custom scenario applies and  Enter a custom scenario. Confirm chip
           stacks                       updates and delta stacks.

  9        Attribution line visible on  Confirm "Generated from 247
           Screen 7                     transactions · Sprout AI" appears
                                        below diagnosis.

  10       No console errors on any     Open DevTools, run full flow. Zero
           screen                       errors, zero warnings.
  -------- ---------------------------- --------------------------------------
