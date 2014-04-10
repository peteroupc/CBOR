/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 3/31/2014
 * Time: 3:18 PM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace PeterO.Mail
{
  internal class NamedAddress {
    private string name;

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
    public string Name {
      get {
        return this.name;
      }
    }

    private Address address;

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
    public Address Address {
      get {
        return this.address;
      }
    }

    private bool isGroup;

    /// <summary>Gets a value indicating whether this represents a group
    /// of addresses.</summary>
    /// <value>Whether this represents a group of addresses.</value>
    public bool IsGroup {
      get {
        return this.isGroup;
      }
    }

    public NamedAddress(string address) : this(address, address) {
    }

    public NamedAddress(string displayName, string address) {
      if (String.IsNullOrEmpty(displayName)) {
        displayName = address;
      }
      if (address == null) {
        throw new ArgumentNullException("address");
      }
      this.name = displayName;
      this.address = new Address(address);
    }

    public NamedAddress(string displayName, string localPart, string domain) {
      if (localPart == null) {
        throw new ArgumentNullException("localPart");
      }
      if (domain == null) {
        throw new ArgumentNullException("domain");
      }
      this.address = new Address(localPart, domain);
      if (String.IsNullOrEmpty(displayName)) {
        // TODO: Implement Address.ToString()
        displayName = this.address.ToString();
      }
      this.name = displayName;
    }

    public NamedAddress(string groupName, IList<NamedAddress> mailboxes) {
      if (groupName == null) {
        throw new ArgumentNullException("groupName");
      }
      if (groupName.Length == 0) {
        throw new ArgumentException("groupName is empty.");
      }
      if (mailboxes == null) {
        throw new ArgumentNullException("mailboxes");
      }
      this.isGroup = true;
      this.name = groupName;
       foreach (NamedAddress mailbox in mailboxes) {
        if (mailbox.IsGroup) {
          throw new ArgumentException("A mailbox in the list is a group");
        }
      }
      this.groupAddresses = new List<NamedAddress>(mailboxes);
    }

    private IList<NamedAddress> groupAddresses;

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
    public IList<NamedAddress> GroupAddresses {
      get {
        return new System.Collections.ObjectModel.ReadOnlyCollection<NamedAddress>(this.groupAddresses);
      }
    }
  }
}
