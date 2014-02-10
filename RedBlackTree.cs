/*
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */
using System;
using System.Collections.Generic;

namespace PeterO
{
    /// <summary>Red-black tree, modified by Peter O. from public-domain
    /// Java code originally written by Doug Lea.</summary>
    /// <typeparam name='T'>Type of each element in the tree.</typeparam>
  internal class RedBlackTree<T> : ICollection<T> {
    private class RBCell {
      private static bool valueRED = false;
      private static bool valueBLACK = true;

      /**
       * The element held in the node
       **/

      private T elementValue;

      /**
       * The node color (valueRED, valueBLACK)
       **/

      private bool colorValue = valueBLACK;

      /**
       * Pointer to left child
       **/

      private RBCell leftValue = null;

      /**
       * Pointer to right child
       **/

      private RBCell rightValue = null;

      /**
       * Pointer to parent (null if root)
       **/

      private RBCell parentValue = null;

      /**
       * Make a new cell with given element, null links, and valueBLACK color.
       * Normally only called to establish a new root.
       **/

      public RBCell(T element) {
        this.elementValue = element;
      }

      /**
       * return the element value
       **/

      public T element() {
 return this.elementValue;
}

      /**
       * set the element value
       **/

      public void element(T v) { this.elementValue = v; }

      /**
       * Return left child (or null)
       **/

      public RBCell left() {
 return this.leftValue;
}

      /**
       * Return right child (or null)
       **/

      public RBCell right() {
 return this.rightValue;
}

      /**
       * Return parent (or null)
       **/
      public RBCell parent() {
 return this.parentValue;
}

      /**
       * Return color of node p, or valueBLACK if p is null
       *
       **/

      internal static bool colorOf(RBCell p) {
 return (p == null) ? valueBLACK : p.colorValue;
}

      /**
       * return parent of node p, or null if p is null
       **/
      internal static RBCell parentOf(RBCell p) {
 return (p == null) ? null : p.parentValue;
}

      /**
       * Set the color of node p, or do nothing if p is null
       **/

      internal static void setColor(RBCell p, bool c) { if (p != null) {
 p.colorValue = c;
} }

      /**
       * return left child of node p, or null if p is null
       **/

      internal static RBCell leftOf(RBCell p) {
 return (p == null) ? null : p.leftValue;
}

      /**
       * return right child of node p, or null if p is null
       **/

      internal static RBCell rightOf(RBCell p) {
 return (p == null) ? null : p.rightValue;
}

      /**
       * Copy all content fields from another node
       * Override this if you add any other fields in subclasses.
       *
       **/

      protected void copyContents(RBCell t) {
        this.elementValue = t.elementValue;
      }

      /**
       * Return the minimum element of the current (sub)tree
       **/

      public RBCell leftmost() {
        RBCell p = this;
        for (; p.leftValue != null; p = p.leftValue) {}
        return p;
      }

      /**
       * Return the maximum element of the current (sub)tree
       **/
      public RBCell rightmost() {
        RBCell p = this;
        for (; p.rightValue != null; p = p.rightValue) {}
        return p;
      }

      /**
       * Return the root (parentless node) of the tree
       **/
      public RBCell root() {
        RBCell p = this;
        for (; p.parentValue != null; p = p.parentValue) {}
        return p;
      }

      /**
       * Return true if node is a root (i.e., has a null parent)
       **/

      public bool isRoot() {
 return this.parentValue == null;
}

      /**
       * Return the inorder successor, or null if no such
       **/

      public RBCell successor() {
        if (this.rightValue != null) {
 return this.rightValue.leftmost();
  } else {
          RBCell p = this.parentValue;
          RBCell ch = this;
          while (p != null && ch == p.rightValue) {
  { ch = p;
} p = p.parentValue; }
          return p;
        }
      }

      /**
       * Return the inorder predecessor, or null if no such
       **/

      public RBCell predecessor() {
        if (this.leftValue != null) {
 return this.leftValue.rightmost();
  } else {
          RBCell p = this.parentValue;
          RBCell ch = this;
          while (p != null && ch == p.leftValue) {
  { ch = p;
} p = p.parentValue; }
          return p;
        }
      }

      /**
       * Return the number of nodes in the subtree
       **/
      public int size() {
        int c = 1;
        if (this.leftValue != null) {
 c += this.leftValue.size();
}
        if (this.rightValue != null) {
 c += this.rightValue.size();
}
        return c;
      }

      /**
       * Return node of current subtree containing element as element(),
       * if it exists, else null.
       * Uses IComparer cmp to find and to check equality.
       **/

      public RBCell find(T element, IComparer<T> cmp) {
        RBCell t = this;
        for (;;) {
          int diff = cmp.Compare(element, t.element());
          if (diff == 0) {
 return t;
  } else if (diff < 0) {
 t = t.leftValue;
  } else {
 t = t.rightValue;
}
          if (t == null) {
 return null;
}
        }
      }

      /**
       * Return number of nodes of current subtree containing element.
       * Uses IComparer cmp to find and to check equality.
       **/
      public int count(T element, IComparer<T> cmp) {
        int c = 0;
        RBCell t = this;
        while (t != null) {
          int diff = cmp.Compare(element, t.element());
          if (diff == 0) {
            ++c;
            if (t.leftValue == null) {
 t = t.rightValue;
  } else if (t.rightValue == null) {
 t = t.leftValue;
  } else {
              c += t.rightValue.count(element, cmp);
              t = t.leftValue;
            }
  } else if (diff < 0) {
 t = t.leftValue;
  } else {
 t = t.rightValue;
}
        }
        return c;
      }

      /**
       * Insert cell as the left child of current node, and then
       * rebalance the tree it is in.
       * @param cell the cell to add
       * @param root, the root of the current tree
       * @return the new root of the current tree. (Rebalancing
       * can change the root!)
       **/

      public RBCell insertLeft(RBCell cell, RBCell root) {
        this.leftValue = cell;
        cell.parentValue = this;
        return cell.fixAfterInsertion(root);
      }

      /**
       * Insert cell as the right child of current node, and then
       * rebalance the tree it is in.
       * @param cell the cell to add
       * @param root, the root of the current tree
       * @return the new root of the current tree. (Rebalancing
       * can change the root!)
       **/

      public RBCell insertRight(RBCell cell, RBCell root) {
        this.rightValue = cell;
        cell.parentValue = this;
        return cell.fixAfterInsertion(root);
      }

      /**
       * Delete the current node, and then rebalance the tree it is in
       * @param root the root of the current tree
       * @return the new root of the current tree. (Rebalancing
       * can change the root!)
       **/

      public RBCell delete(RBCell root) {
        // if strictly internal, swap contents with successor and then delete it
        if (this.leftValue != null && this.rightValue != null) {
          RBCell s = this.successor();
          this.copyContents(s);
          return s.delete(root);
        }

        // Start fixup at replacement node, if it exists
        RBCell replacement = (this.leftValue != null) ? this.leftValue : this.rightValue;

        if (replacement != null) {
          // link replacement to parent
          replacement.parentValue = this.parentValue;
          if (this.parentValue == null) {
 root = replacement;
  } else if (this == this.parentValue.leftValue) {
 this.parentValue.leftValue = replacement;
  } else {
 this.parentValue.rightValue = replacement;
}

          // null out links so they are OK to use by fixAfterDeletion
          this.leftValue = null;
          this.rightValue = null;
          this.parentValue = null;

          // fix replacement
          if (this.colorValue == valueBLACK) {
 root = replacement.fixAfterDeletion(root);
}
          return root;
  } else if (this.parentValue == null)  // exit if we are the only node
          return null;

        else {  // if no children, use self as phantom replacement and then unlink

          if (this.colorValue == valueBLACK) {
 root = this.fixAfterDeletion(root);
}

          // Unlink (Couldn't before since fixAfterDeletion needs parent ptr)
          if (this.parentValue != null) {
            if (this == this.parentValue.leftValue) {
 this.parentValue.leftValue = null;
  } else if (this == this.parentValue.rightValue) {
 this.parentValue.rightValue = null;
}
            this.parentValue = null;
          }

          return root;
        }
      }

      /** From CLR **/
      protected RBCell rotateLeft(RBCell root) {
        RBCell r = this.rightValue;
        this.rightValue = r.leftValue;
        if (r.leftValue != null) {
 r.leftValue.parentValue = this;
}
        r.parentValue = this.parentValue;
        if (this.parentValue == null) {
 root = r;
  } else if (this.parentValue.leftValue == this) {
 this.parentValue.leftValue = r;
  } else {
 this.parentValue.rightValue = r;
}
        r.leftValue = this;
        this.parentValue = r;
        return root;
      }

      /** From CLR **/
      protected RBCell rotateRight(RBCell root) {
        RBCell l = this.leftValue;
        this.leftValue = l.rightValue;
        if (l.rightValue != null) {
 l.rightValue.parentValue = this;
}
        l.parentValue = this.parentValue;
        if (this.parentValue == null) {
 root = l;
  } else if (this.parentValue.rightValue == this) {
 this.parentValue.rightValue = l;
  } else {
 this.parentValue.leftValue = l;
}
        l.rightValue = this;
        this.parentValue = l;
        return root;
      }

      /** From CLR **/
      private RBCell fixAfterInsertion(RBCell root) {
        this.colorValue = valueRED;
        RBCell x = this;

        while (x != null && x != root && x.parentValue.colorValue == valueRED) {
          if (parentOf(x) == leftOf(parentOf(parentOf(x)))) {
            RBCell y = rightOf(parentOf(parentOf(x)));

            if (colorOf(y) == valueRED) {
              setColor(parentOf(x), valueBLACK);
              setColor(y, valueBLACK);
              setColor(parentOf(parentOf(x)), valueRED);
              x = parentOf(parentOf(x));
  } else {
              if (x == rightOf(parentOf(x))) {
                x = parentOf(x);
                root = x.rotateLeft(root);
              }
              setColor(parentOf(x), valueBLACK);
              setColor(parentOf(parentOf(x)), valueRED);
              if (parentOf(parentOf(x)) != null) {
 root = parentOf(parentOf(x)).rotateRight(root);
}
            }
          }

          else {
            RBCell y = leftOf(parentOf(parentOf(x)));

            if (colorOf(y) == valueRED) {
              setColor(parentOf(x), valueBLACK);
              setColor(y, valueBLACK);
              setColor(parentOf(parentOf(x)), valueRED);
              x = parentOf(parentOf(x));
  } else {
              if (x == leftOf(parentOf(x))) {
                x = parentOf(x);
                root = x.rotateRight(root);
              }
              setColor(parentOf(x), valueBLACK);
              setColor(parentOf(parentOf(x)), valueRED);
              if (parentOf(parentOf(x)) != null) {
 root = parentOf(parentOf(x)).rotateLeft(root);
}
            }
          }
        }
        root.colorValue = valueBLACK;
        return root;
      }
      /** From CLR **/
      private RBCell fixAfterDeletion(RBCell root) {
        RBCell x = this;
        while (x != root && colorOf(x) == valueBLACK) {
          if (x == leftOf(parentOf(x))) {
            RBCell sib = rightOf(parentOf(x));

            if (colorOf(sib) == valueRED) {
              setColor(sib, valueBLACK);
              setColor(parentOf(x), valueRED);
              root = parentOf(x).rotateLeft(root);
              sib = rightOf(parentOf(x));
            }

            if (colorOf(leftOf(sib)) == valueBLACK && colorOf(rightOf(sib)) == valueBLACK) {
              setColor(sib, valueRED);
              x = parentOf(x);
  } else {
              if (colorOf(rightOf(sib)) == valueBLACK) {
                setColor(leftOf(sib), valueBLACK);
                setColor(sib, valueRED);
                root = sib.rotateRight(root);
                sib = rightOf(parentOf(x));
              }
              setColor(sib, colorOf(parentOf(x)));
              setColor(parentOf(x), valueBLACK);
              setColor(rightOf(sib), valueBLACK);
              root = parentOf(x).rotateLeft(root);
              x = root;
            }
          }

          else {  // symmetric

            RBCell sib = leftOf(parentOf(x));

            if (colorOf(sib) == valueRED) {
              setColor(sib, valueBLACK);
              setColor(parentOf(x), valueRED);
              root = parentOf(x).rotateRight(root);
              sib = leftOf(parentOf(x));
            }

            if (colorOf(rightOf(sib)) == valueBLACK && colorOf(leftOf(sib)) == valueBLACK) {
              setColor(sib, valueRED);
              x = parentOf(x);
  } else {
              if (colorOf(leftOf(sib)) == valueBLACK) {
                setColor(rightOf(sib), valueBLACK);
                setColor(sib, valueRED);
                root = sib.rotateLeft(root);
                sib = leftOf(parentOf(x));
              }
              setColor(sib, colorOf(parentOf(x)));
              setColor(parentOf(x), valueBLACK);
              setColor(leftOf(sib), valueBLACK);
              root = parentOf(x).rotateRight(root);
              x = root;
            }
          }
        }
        setColor(x, valueBLACK);
        return root;
      }
    }

    // instance variables

    /**
     * The root of the tree. Null if empty.
     **/

    private RBCell treeValue;

    /**
     * The comparator to use for ordering.
     **/
    protected IComparer<T> cmpValue;

    // constructors

    /**
     * Make an empty tree.
     * Initialize to use DefaultIComparer for ordering
     **/
    public RedBlackTree() : this(null, null, 0) {}

    /**
     * Make an empty tree, using the supplied element comparator for ordering.
     **/
    public RedBlackTree(IComparer<T> c) : this(c, null, 0) {}

    /**
     * Special version of constructor needed by clone()
     **/

    private RedBlackTree(IComparer<T> cmp, RBCell t, int n) {
      this.countValue = n;
      this.treeValue = t;
      if (cmp != null) {
 this.cmpValue = cmp;
  } else {
 this.cmpValue = Comparer<T>.Default;
}
    }

    // Collection methods

    /**
     * Implements collections.Collection.includes.
     * Time complexity: O(log n).
     * @see collections.Collection#includes
     **/
    public bool Contains(T element) {
      if (this.countValue == 0) {
 return false;
}
      return this.treeValue.find(element, this.cmpValue) != null;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='element'>A T object.</param>
    /// <param name='outval'>A T object. (2).</param>
    /// <returns>A Boolean object.</returns>
public bool Find(T element, out T outval) {
      if (this.countValue == 0) {
        outval = default(T);
        return false;
      }
      RBCell cell = this.treeValue.find(element, this.cmpValue);
      if (cell == null) {
        outval = default(T);
        return false;
      }
      outval = cell.element();
      return true;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='element'>A T object.</param>
    /// <returns>A 32-bit signed integer.</returns>
public int OccurrencesOf(T element) {
      if (this.countValue == 0) {
 return 0;
}
      return this.treeValue.count(element, this.cmpValue);
    }

    /**
     * Implements collections.UpdatableCollection.clear.
     * Time complexity: O(1).
     * @see collections.UpdatableCollection#clear
     **/
    public void Clear() {
      this.countValue = 0;
      this.treeValue = null;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='element'>A T object.</param>
public void RemoveAll(T element) {
      this.remove_(element, true);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='element'>A T object.</param>
    /// <returns>A Boolean object.</returns>
public bool Remove(T element) {
      return this.remove_(element, false);
    }

    /**
     * Implements collections.UpdatableCollection.take.
     * Time complexity: O(log n).
     * Takes the least element.
     * @see collections.UpdatableCollection#take
     **/
    public T Pop() {
      if (this.countValue != 0) {
        RBCell p = this.treeValue.leftmost();
        T v = p.element();
        this.treeValue = p.delete(this.treeValue);
        this.decCount();
        return v;
      }
      return default(T);
    }

    private enum OccurrenceMode {
      AlwaysAdd,
      AddIfMissing,
      OverwriteIfExisting
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='element'>A T object.</param>
    /// <returns>A Boolean object.</returns>
public bool AddIfMissing(T element) {
      return this.addInternal(element, OccurrenceMode.AddIfMissing);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='element'>A T object.</param>
    /// <returns>A Boolean object.</returns>
public bool AddOverwrite(T element) {
      return this.addInternal(element, OccurrenceMode.OverwriteIfExisting);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='element'>A T object.</param>
public void Add(T element) {
      this.addInternal(element, OccurrenceMode.AlwaysAdd);
    }

    // helper methods
    private bool addInternal(T element, OccurrenceMode checkOccurrence) {
      if (this.treeValue == null) {
        this.treeValue = new RBCell(element);
        this.incCount();
  } else {
        RBCell t = this.treeValue;
        for (;;) {
          int diff = this.cmpValue.Compare(element, t.element());
          if (diff == 0 && checkOccurrence == OccurrenceMode.AddIfMissing) {
 return false;
}
          if (diff == 0 && checkOccurrence == OccurrenceMode.OverwriteIfExisting) {
            t.element(element);
            return false;
  } else if (diff <= 0) {
            if (t.left() != null) {
 t = t.left();
  } else {
              this.treeValue = t.insertLeft(new RBCell(element), this.treeValue);
              this.incCount();
              return true;
            }
  } else {
            if (t.right() != null) {
 t = t.right();
  } else {
              this.treeValue = t.insertRight(new RBCell(element), this.treeValue);
              this.incCount();
              return true;
            }
          }
        }
      }
      return true;
    }

    private bool remove_(T element, bool allOccurrences) {
      bool ret = false;
      while (this.countValue > 0) {
        RBCell p = this.treeValue.find(element, this.cmpValue);
        if (p != null) {
          this.treeValue = p.delete(this.treeValue);
          this.decCount();
          ret = true;
          if (!allOccurrences) {
 return ret;
}
  } else {
 break;
}
      }
      return ret;
    }

    private IEnumerable<T> Iterator() {
      if (this.treeValue != null) {
        RBCell t = this.treeValue.leftmost();
        while (t != null) {
          T v = t.element();
          yield return v;
          t = t.successor();
        }
      }
    }

    private int countValue;

    private void incCount() {
      ++this.countValue;
    }

    private void decCount() {
      --this.countValue;
    }

    /// <summary/><value></value>
public int Count {
      get {
        return this.countValue;
      }
    }

    /// <summary/><value></value>
public bool IsReadOnly {
      get {
        return false;
      }
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='array'>A T[] object.</param>
    /// <param name='arrayIndex'>A 32-bit signed integer.</param>
public void CopyTo(T[] array, int arrayIndex) {
      if (this.treeValue != null) {
        RBCell t = this.treeValue.leftmost();
        while (t != null && arrayIndex < array.Length) {
          T v = t.element();
          if (arrayIndex >= 0 && arrayIndex < array.Length) {
            array[arrayIndex] = v;
          }
          ++arrayIndex;
          t = t.successor();
        }
      }
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>An IEnumerator(T) object.</returns>
public IEnumerator<T> GetEnumerator() {
      return this.Iterator().GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return this.Iterator().GetEnumerator();
    }
  }
}
