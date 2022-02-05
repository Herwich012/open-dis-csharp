// Copyright (c) 1995-2009 held by the author(s).  All rights reserved.
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions
// are met:
// * Redistributions of source code must retain the above copyright
//    notice, this list of conditions and the following disclaimer.
// * Redistributions in binary form must reproduce the above copyright
//   notice, this list of conditions and the following disclaimer
//   in the documentation and/or other materials provided with the
//   distribution.
// * Neither the names of the Naval Postgraduate School (NPS)
//   Modeling Virtual Environments and Simulation (MOVES) Institute
//   (http://www.nps.edu and http://www.MovesInstitute.org)
//   nor the names of its contributors may be used to endorse or
//   promote products derived from this software without specific
//   prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// AS IS AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
// FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
// COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
// INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
// BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
// LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
// POSSIBILITY OF SUCH DAMAGE.
//
// Copyright (c) 2008, MOVES Institute, Naval Postgraduate School. All 
// rights reserved. This work is licensed under the BSD open source license,
// available at https://www.movesinstitute.org/licenses/bsd.html
//
// Author: DMcG
// Modified for use with C#:
//  - Peter Smith (Naval Air Warfare Center - Training Systems Division)
//  - Zvonko Bostjancic (Blubit d.o.o. - zvonko.bostjancic@blubit.si)

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;
using OpenDis.Core;

namespace OpenDis.Dis1998
{
    /// <summary>
    /// Section 5.3.12.4: Stop freeze simulation, relaible. COMPLETE
    /// </summary>
    [Serializable]
    [XmlRoot]
    [XmlInclude(typeof(ClockTime))]
    public partial class StopFreezeReliablePdu : SimulationManagementWithReliabilityFamilyPdu, IEquatable<StopFreezeReliablePdu>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StopFreezeReliablePdu"/> class.
        /// </summary>
        public StopFreezeReliablePdu()
        {
            PduType = 54;
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>
        ///    <c>true</c> if operands are not equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(StopFreezeReliablePdu left, StopFreezeReliablePdu right) => !(left == right);

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>
        ///    <c>true</c> if both operands are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(StopFreezeReliablePdu left, StopFreezeReliablePdu right)
            => ReferenceEquals(left, right) || (left is not null && right is not null && left.Equals(right));

        public override int GetMarshalledSize()
        {
            int marshalSize = base.GetMarshalledSize();
            marshalSize += RealWorldTime.GetMarshalledSize();  // this._realWorldTime
            marshalSize += 1;  // this._reason
            marshalSize += 1;  // this._frozenBehavior
            marshalSize += 1;  // this._requiredReliablityService
            marshalSize += 1;  // this._pad1
            marshalSize += 4;  // this._requestID
            return marshalSize;
        }

        /// <summary>
        /// Gets or sets the time in real world for this operation to happen
        /// </summary>
        [XmlElement(Type = typeof(ClockTime), ElementName = "realWorldTime")]
        public ClockTime RealWorldTime { get; set; } = new ClockTime();

        /// <summary>
        /// Gets or sets the Reason for stopping/freezing simulation
        /// </summary>
        [XmlElement(Type = typeof(byte), ElementName = "reason")]
        public byte Reason { get; set; }

        /// <summary>
        /// Gets or sets the internal behvior of the simulation while frozen
        /// </summary>
        [XmlElement(Type = typeof(byte), ElementName = "frozenBehavior")]
        public byte FrozenBehavior { get; set; }

        /// <summary>
        /// Gets or sets the reliablity level
        /// </summary>
        [XmlElement(Type = typeof(byte), ElementName = "requiredReliablityService")]
        public byte RequiredReliablityService { get; set; }

        /// <summary>
        /// Gets or sets the padding
        /// </summary>
        [XmlElement(Type = typeof(byte), ElementName = "pad1")]
        public byte Pad1 { get; set; }

        /// <summary>
        /// Gets or sets the Request ID
        /// </summary>
        [XmlElement(Type = typeof(uint), ElementName = "requestID")]
        public uint RequestID { get; set; }

        ///<inheritdoc/>
        public override void MarshalAutoLengthSet(DataOutputStream dos)
        {
            // Set the length prior to marshalling data
            Length = (ushort)GetMarshalledSize();
            Marshal(dos);
        }

        /// <inheritdoc/>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Due to ignoring errors.")]
        public override void Marshal(DataOutputStream dos)
        {
            base.Marshal(dos);
            if (dos != null)
            {
                try
                {
                    RealWorldTime.Marshal(dos);
                    dos.WriteUnsignedByte(Reason);
                    dos.WriteUnsignedByte(FrozenBehavior);
                    dos.WriteUnsignedByte(RequiredReliablityService);
                    dos.WriteUnsignedByte(Pad1);
                    dos.WriteUnsignedInt(RequestID);
                }
                catch (Exception e)
                {
                    if (TraceExceptions)
                    {
                        Trace.WriteLine(e);
                        Trace.Flush();
                    }

                    RaiseExceptionOccured(e);

                    if (ThrowExceptions)
                    {
                        throw;
                    }
                }
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Due to ignoring errors.")]
        public override void Unmarshal(DataInputStream dis)
        {
            base.Unmarshal(dis);

            if (dis != null)
            {
                try
                {
                    RealWorldTime.Unmarshal(dis);
                    Reason = dis.ReadUnsignedByte();
                    FrozenBehavior = dis.ReadUnsignedByte();
                    RequiredReliablityService = dis.ReadUnsignedByte();
                    Pad1 = dis.ReadUnsignedByte();
                    RequestID = dis.ReadUnsignedInt();
                }
                catch (Exception e)
                {
                    if (TraceExceptions)
                    {
                        Trace.WriteLine(e);
                        Trace.Flush();
                    }

                    RaiseExceptionOccured(e);

                    if (ThrowExceptions)
                    {
                        throw;
                    }
                }
            }
        }

        /// <inheritdoc/>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Due to ignoring errors.")]
        public override void Reflection(StringBuilder sb)
        {
            sb.AppendLine("<StopFreezeReliablePdu>");
            base.Reflection(sb);
            try
            {
                sb.AppendLine("<realWorldTime>");
                RealWorldTime.Reflection(sb);
                sb.AppendLine("</realWorldTime>");
                sb.AppendLine("<reason type=\"byte\">" + Reason.ToString(CultureInfo.InvariantCulture) + "</reason>");
                sb.AppendLine("<frozenBehavior type=\"byte\">" + FrozenBehavior.ToString(CultureInfo.InvariantCulture) + "</frozenBehavior>");
                sb.AppendLine("<requiredReliablityService type=\"byte\">" + RequiredReliablityService.ToString(CultureInfo.InvariantCulture) + "</requiredReliablityService>");
                sb.AppendLine("<pad1 type=\"byte\">" + Pad1.ToString(CultureInfo.InvariantCulture) + "</pad1>");
                sb.AppendLine("<requestID type=\"uint\">" + RequestID.ToString(CultureInfo.InvariantCulture) + "</requestID>");
                sb.AppendLine("</StopFreezeReliablePdu>");
            }
            catch (Exception e)
            {
                if (TraceExceptions)
                {
                    Trace.WriteLine(e);
                    Trace.Flush();
                }

                RaiseExceptionOccured(e);

                if (ThrowExceptions)
                {
                    throw;
                }
            }
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) => this == obj as StopFreezeReliablePdu;

        ///<inheritdoc/>
        public bool Equals(StopFreezeReliablePdu obj)
        {
            if (obj.GetType() != GetType())
            {
                return false;
            }

            bool ivarsEqual = base.Equals(obj);
            if (!RealWorldTime.Equals(obj.RealWorldTime))
            {
                ivarsEqual = false;
            }

            if (Reason != obj.Reason)
            {
                ivarsEqual = false;
            }

            if (FrozenBehavior != obj.FrozenBehavior)
            {
                ivarsEqual = false;
            }

            if (RequiredReliablityService != obj.RequiredReliablityService)
            {
                ivarsEqual = false;
            }

            if (Pad1 != obj.Pad1)
            {
                ivarsEqual = false;
            }

            if (RequestID != obj.RequestID)
            {
                ivarsEqual = false;
            }

            return ivarsEqual;
        }

        /// <summary>
        /// HashCode Helper
        /// </summary>
        /// <param name="hash">The hash value.</param>
        /// <returns>The new hash value.</returns>
        private static int GenerateHash(int hash) => hash << (5 + hash);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int result = 0;

            result = GenerateHash(result) ^ base.GetHashCode();

            result = GenerateHash(result) ^ RealWorldTime.GetHashCode();
            result = GenerateHash(result) ^ Reason.GetHashCode();
            result = GenerateHash(result) ^ FrozenBehavior.GetHashCode();
            result = GenerateHash(result) ^ RequiredReliablityService.GetHashCode();
            result = GenerateHash(result) ^ Pad1.GetHashCode();
            result = GenerateHash(result) ^ RequestID.GetHashCode();

            return result;
        }
    }
}
