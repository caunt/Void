﻿using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Encryption;

namespace Void.Proxy.Plugins.ExamplePlugin.Services;

public class DebugService : IEventListener
{
    [Subscribe]
    public void OnSearchServerPrivateKey(SearchServerPrivateKey @event)
    {
        if (@event.Server.Name is not "lobby")
            return;

        @event.Result = Convert.FromHexString("30820276020100300d06092a864886f70d0101010500048202603082025c020100028181009922b100a7d1ac0d7e82de8bc56b939cb356b0e3d4616dccc91f1e2f38b83fd5d53ce14793dbda1e4d5c421d064cb6e1021d719efdac99031d3d5a44959b190d2c57e82922c5e8b21475d296e0cf9bcb63e4b71006a053faec067609ddd6070f4e3ab132356c9f5ebf02ace59c638fc3ba5566d909d4f8b25a722e51f3370f1f0203010001028180015dc0f7ead42dc2497629a4543179cf5e3109e8c67332e04859e038c3d4e1ca844cd0a61ebbafe7cab44c4fe7612f3c1bae38f7fc76cd3f135dcea976956f2844d1095b1066921da14fa5d0fa086c78e1c9290a418a4b83a443c7fc0c244ad158d5de3e3f61f0fb43714a90d60e841e3072aef1c5d49abc0e7f6ba9c7218ce5024100f50b75a4650f5f5b0dca0736bf5893274e33c07377f2ee690c2c0554b435e3a4b539e2e74a9a858af43dc373621bff3880bbc3f862bf6a0ceab6557ce3afa40b0241009ffb544ed1c01e288f759432b4c05311de11733611d44bcf2e157036a95e0b24b24d9c5f7bec41514ecb903443c1f318ec127b2443cb45c3ad2be7b81f5db9bd024004172ae191a64e0b3111bc4917ac9cd83d1ff408796a1ebcb62d4df17a08a4422d9c47c360a56fb1401e7fd2bf4284622713fb537c2281dcb15655dafdda02bd0240291b03ab88db4b2472cfec3727182f7fecc6210b28839ac2edfd562ac553c39b373117d7b4d89c63ced221083cccadb09e9f95025964f654f3becf3810df1d25024100d1e8cc21a13a5e2e902080010751acda11220fbcfe5d6e455b24a9b032634719a61de55f159ea59883bff7ddc5c4635b21c6bd529d104218d830b5f9cb7ed393");
    }
}