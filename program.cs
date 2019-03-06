using System;
using System.Threading.Tasks;
using NBitcoin;
using NBitcoin.RPC;

namespace calc_balance
{
  class Program
  {
    static void Main(string[] args)
    {
      Task.Run(async () => {
        RPCClient client = new RPCClient("user:123456","http://localhost:18443",Network.RegTest);
        Money balance = await client.GetBalanceAsync();
        Console.WriteLine("getbalance => {0}", balance.Satoshi);
        UnspentCoin[] coins = await client.ListUnspentAsync();
        long amount = 0;
        foreach(var coin in coins){
          amount += coin.Amount.Satoshi;
        }
        Console.WriteLine("unspent acc => {0}",amount);
        
        if(balance.Equals(Money.Satoshis(amount))) Console.WriteLine("verified successfully!");
        else Console.WriteLine("failed to verify balance");
      }).Wait();
    }
  }
}
