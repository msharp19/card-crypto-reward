## Details

This is a .NET 6 Web API to handle crypto credit card rewards. This is in its infancy and I plan to extend this over the coming months. 

## Features

- Make crypto reward payments for credit card spend
- Uses no 3rd party wallets (all direct)
- Can add crypto currencies as long as there is an implementation ie. I can add USDT using the Ethereum implementation (enum selection) because it uses the same RPC methods.
- Users can select users rewards (% mix and match for currencies) and update these
- Can set/edit spend bands to define how much crypto is given for what currency based on monthly credit card spend
- Can add/update system wallets (deposits/withdrawals/staking)
- Creates transactions and fee transactions for deposit/withdrawals/staking
- Creates instructions to process transactions which are picked up currently by hosted services (migrating to webjobs)
- Whitelist client wallets to withdraw to
- Manage users
- Manage wallets

## Notes

- This currently only has implementation for Bitcoin and Ethereum but many others can be added pretty easily
- There are a few bugs thats need to be fixed (all have workarounds at the moment)
- Credit card payment checks are stubbed out (returns random monthly spend when queried)
- No authentication enforced yet (planning to add Identity server as another project and have OIDC). The project is setup to accept this.
- Am going to remove QBitNinja implementation (is currently commented out)
- Ive added Azure webjobs where the hosted services can be moved to if required (plan is to use the jobs NOT hosted services)
