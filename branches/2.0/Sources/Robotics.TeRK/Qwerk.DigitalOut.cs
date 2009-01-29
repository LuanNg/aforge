﻿// AForge TeRK Robotics Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2007-2008
// andrew.kirillov@aforgenet.com
//

using TeRKIceLib = TeRK;

namespace AForge.Robotics.TeRK
{
    using System;

    public partial class Qwerk
    {
        /// <summary>
        /// Provides access to Qwerk's digital outputs.
        /// </summary>
        /// 
        /// <remarks><para>The class allows to manipulate 4 Qwerk's digital outputs.</para>
        /// 
        /// <para>Sample usage:</para>
        /// <code>
        /// // get Qwerk's digital outs service
        /// Qwerk.DigitalOut digitalOuts = qwerk.GetDigitalOutService( );
        /// // disbale all outputs
        /// digitalOuts.SetOutputs( false );
        /// // enable zero output
        /// digitalOuts.SetOutput( 0, true );
        /// // enable 2nd and 3rd outputs
        /// bool[] mask = new bool[4] { false, false, true, true };
        /// bool[] states = new bool[4] { false, false, true, true };
        /// digitalOuts.SetOutputs( mask, states );
        /// </code>
        /// </remarks>
        /// 
        public class DigitalOut
        {
            // Qwerk's digital out controller
            private TeRKIceLib.DigitalOutControllerPrx digitalOutController = null;

            /// <summary>
            /// Initializes a new instance of the <see cref="Qwerk.DigitalOut"/> class.
            /// </summary>
            /// 
            /// <param name="qwerk">Reference to <see cref="Qwerk"/> object, which is connected to Qwerk board.</param>
            /// 
            /// <exception cref="NotConnectedException">The passed reference to <see cref="Qwerk"/> object is not connected to
            /// Qwerk board.</exception>
            /// <exception cref="ConnectFailedException">Failed connecting to the requested service.</exception>
            /// <exception cref="ServiceAccessFailedException">Failed accessing to the requested service.</exception>
            /// 
            public DigitalOut( Qwerk qwerk )
            {
                string hostAddress = qwerk.HostAddress;

                // check if connection was established
                if ( hostAddress != null )
                {
                    try
                    {
                        Ice.ObjectPrx obj = qwerk.iceCommunicator.stringToProxy( "'::TeRK::DigitalOutController':tcp -h " + hostAddress + " -p 10101" );
                        digitalOutController = TeRKIceLib.DigitalOutControllerPrxHelper.checkedCast( obj );
                    }
                    catch ( Ice.ObjectNotExistException )
                    {
                        // the object does not exist on the host
                        throw new ServiceAccessFailedException( "Failed accessing to the requested service." );
                    }
                    catch
                    {
                        throw new ConnectFailedException( "Failed connecting to the requested service." );
                    }

                    if ( digitalOutController == null )
                    {
                        throw new ServiceAccessFailedException( "Failed accessing to the requested cervice." );
                    }
                }
                else
                {
                    throw new NotConnectedException( "Qwerk object is not connected to a board." );
                }
            }

            /// <summary>
            /// Set state of specified digital output.
            /// </summary>
            /// 
            /// <param name="output">Digital output to state for, [0, 3].</param>
            /// <param name="state">Digital output's state to set.</param>
            /// 
            /// <remarks><para>The method sets state of one of Qwerk's digital outputs, which index is
            /// specified. The output is either enabled (state is set to <see langword="true"/> or
            /// disabled otherwise.</para></remarks>
            /// 
            /// <exception cref="ArgumentOutOfRangeException">Invalid output is specified.</exception>
            /// <exception cref="NotConnectedException">No connection to Qwerk or its service.</exception>
            /// <exception cref="ConnectionLostException">Connestion to Qwerk is lost.</exception>
            /// 
            public void SetOutput( int output, bool state )
            {
                if ( ( output < 0 ) || ( output > 3 ) )
                {
                    throw new ArgumentOutOfRangeException( "Invalid output is specified." );
                }

                bool[] outputsMask = new bool[4];
                bool[] states = new bool[4];

                outputsMask[output] = true;
                states[output] = state;

                SetOutputs( outputsMask, states );
            }

            /// <summary>
            /// Set state of all digital outputs.
            /// </summary>
            /// 
            /// <param name="state">State, which will be set to all digital outputs.</param>
            /// 
            /// <remarks><para>The method sets the same state to all Qwerk's digital outputs.</para></remarks>
            /// 
            /// <exception cref="NotConnectedException">No connection to Qwerk or its service.</exception>
            /// <exception cref="ConnectionLostException">Connestion to Qwerk is lost.</exception>
            /// 
            public void SetOutputs( bool state )
            {
                bool[] outputsMask = new bool[4];
                bool[] states = new bool[4];

                for ( int i = 0; i < 4; i++ )
                {
                    outputsMask[i] = true;
                    states[i] = state;
                }

                SetOutputs( outputsMask, states );
            }

            /// <summary>
            /// Set state of all digital outputs.
            /// </summary>
            /// 
            /// <param name="outpusMask">Mask array specifying which output's state needs to be set.</param>
            /// <param name="states">Array of outputs' states.</param>
            /// 
            /// <remarks><para>The <paramref name="outpusMask"/> and <paramref name="states"/> arrays specify
            /// which Qwerk's digital output's state should be updated. If value of the <paramref name="outpusMask"/>
            /// array is set to <see langword="true"/>, then corresponding output's state is changed to the state,
            /// which is specified in <paramref name="states"/> array.</para>
            /// </remarks>
            /// 
            /// <exception cref="ArgumentException">Incorrect length of outputs' masks or states array.</exception>
            /// <exception cref="NotConnectedException">No connection to Qwerk or its service.</exception>
            /// <exception cref="ConnectionLostException">Connestion to Qwerk is lost.</exception>
            /// 
            public void SetOutputs( bool[] outpusMask, bool[] states )
            {
                if ( ( outpusMask.Length != 4 ) || ( states.Length != 4 ) )
                {
                    throw new ArgumentException( "Incorrect length of output' masks or states array." );
                }

                // check controller
                if ( digitalOutController == null )
                {
                    throw new NotConnectedException( "Qwerk's service is not connected." );
                }

                try
                {
                    // execute leds' command
                    digitalOutController.execute( new TeRKIceLib.DigitalOutCommand( outpusMask, states ) );
                }
                catch
                {
                    throw new ConnectionLostException( "Connection is lost." );
                }
            }
        }
    }
}