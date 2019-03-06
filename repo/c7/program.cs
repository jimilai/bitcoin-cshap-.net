using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using NBitcoin;
using NBitcoin.SPV;
using NBitcoin.Protocol;
using NBitcoin.RPC;

#pragma warning disable CS0612

namespace wallet_balance
{
  class Program
  {
    static Network network = Network.RegTest;
    
    static void Main(string[] args)
    {
      ExtKey masterKey = new ExtKey();
      WalletCreation wc = new WalletCreation
      {
        Network = network,
        RootKeys = new []{ masterKey.Neuter() }
      };
      Wallet wallet = new Wallet(wc);
      wallet.Configure();
      
      NetworkAddress na = new NetworkAddress(IPAddress.Parse("127.0.0.1"),18444);
      wallet.AddressManager.Add(na);
      wallet.Group.MaximumNodeConnection = 1;
      wallet.NewWalletTransaction += (Wallet sender, WalletTransaction wtx) => 
      {
        WalletTransactionsCollection wtxc = wallet.GetTransactions();
        Console.WriteLine("wallet tx count => {0}", wtxc.Count);
         Console.WriteLine("immature => {0}", wtxc.Summary.Immature.Amount);
         Console.WriteLine("confirmed => {0}", wtxc.Summary.Confirmed.Amount);
         Console.WriteLine("unconfirmed => {0}", wtxc.Summary.UnConfirmed.Amount);
         Console.WriteLine("spendable => {0}", wtxc.Summary.Spendable.Amount);
      };
      wallet.Connect();
      
      BitcoinAddress addr = wallet.GetNextScriptPubKey().GetDestinationAddress(network);
      Console.WriteLine("receiver address => {0}", addr);
      
      Task.Run(() => {
        Console.WriteLine("wait a while before trigger tx ...");
        Thread.Sleep(2000);
        RPCClient client = new RPCClient("user:123456","http://localhost:18443",network);
        uint256 txid = client.SendToAddress(addr,Money.Coins(0.1m));
        Console.WriteLine("trigger txid => {0}",txid);
        client.Generate(1);
      });
      
      Console.ReadLine();
    }
  }
}
