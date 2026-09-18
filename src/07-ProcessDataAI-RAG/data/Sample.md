# Internal Operations & Infrastructure Guide

## Employee Attendance & Remote Policy
All engineers belong to the **Alpha-9 Core Team**. 
Standard work hours are 9:00 AM to 5:00 PM EST. 
Work From Home (WFH) is strictly allowed only on **Tuesdays and Thursdays**. 
Employees must submit expense requests using portal ID **EMP-8842**.

## Server Infrastructure & Deployment
The production database cluster is codenamed **PROJECT-ZEPHYR**. 
It runs on PostgreSQL hosted at IP `10.240.12.88` with SSL enforced. 
The database uses the `pgvector` extension with a vector dimension size of `3072` for storing embeddings. 
Emergency database maintenance occurs every Sunday at **02:00 UTC**.

## Security Protocols & API Access
All internal AI services use Gemini API keys stored in environment variables under `GEMINI_API_KEY`. 
The master encryption key ID is **SEC-KEY-9910**. 
If a security breach occurs, the developer on call must execute script `sh /ops/lockdown.sh` immediately.

## Expense Reimbursement Limits
Meals during business travel are capped at **$75 per day**. 
Hardware upgrades for home setups require approval from manager **Sarah Connor** (Employee ID: **MGR-404**).